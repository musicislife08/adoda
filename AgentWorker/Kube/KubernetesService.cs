using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AgentWorker.AzureDevOps;
using AgentWorker.Utilities;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;

namespace AgentWorker.Kube
{
    public interface IKubernetesService
    {
        Task<string> CreateAgentAsync();
        Task<bool> DeletePodAsync(string podName);
        Task<List<string>> GetPodsAsync();
    }

    // TODO: Implement Logging
    public class KubernetesService : IKubernetesService
    {
        private readonly Kubernetes _client;
        private readonly ILogger<KubernetesService> _logger;
        private readonly KubernetesConfig _config;
        private readonly AdoConfig _adoConfig;

        public KubernetesService(
            ILogger<KubernetesService> logger,
            IOptions<KubernetesConfig> options,
            IOptions<AdoConfig> adoOptions)
        {
            _config = options.Value;
            _adoConfig = adoOptions.Value;
            _logger = logger;
            _logger.LogDebug("{ServiceName} Initializing", nameof(KubernetesService));
            var cfg = string.IsNullOrWhiteSpace(_config.LocalKubeconfigPath)
                ? KubernetesClientConfiguration.InClusterConfig()
                : KubernetesClientConfiguration.BuildConfigFromConfigFile(_config.LocalKubeconfigPath,
                    _config.KubeconfigContextName);
            _client = new Kubernetes(cfg);
            Initialize().ConfigureAwait(false);
        }

        public async Task<string> CreateAgentAsync()
        {
            _logger.LogDebug("Getting Agent Spec in {Namespace} Namespace", _config.AgentNamespace);
            var agentSpec = await GetAgentSpecAsync(_config.AgentNamespace);
            var podSpec = await Templates.ReadPodTemplateAsync();
            var containerSpec = podSpec.Spec.Containers.First();
            var name = agentSpec.GeneratePodName();
            // Set Properties
            podSpec.Metadata.Name = name;
            podSpec.Metadata.NamespaceProperty = _config.AgentNamespace;
            containerSpec.Name = name;
            containerSpec.Image = agentSpec.GetImageName();
            containerSpec.Env ??= new List<V1EnvVar>();
            containerSpec.Env.Add(new V1EnvVar("AZP_POOL", _adoConfig.AgentPool));
            containerSpec.Env.Add(new V1EnvVar("AZP_URL", _adoConfig.OrgUri));
            containerSpec.Env.Add(new V1EnvVar
            {
                ValueFrom = new V1EnvVarSource(null, null, null, new V1SecretKeySelector("pat", "azdo-token")),
                Name = "AZP_TOKEN"
            });
            var createResult = await _client.CreateNamespacedPodWithHttpMessagesAsync(podSpec, _config.AgentNamespace);
            if (!createResult.Response.IsSuccessStatusCode)
                throw new HttpRequestException(createResult.Response.ReasonPhrase);
            return name;
        }

        public async Task<List<string>> GetPodsAsync()
        {
            var result = await _client.ListNamespacedPodWithHttpMessagesAsync(_config.AgentNamespace);
            if (result.Response.IsSuccessStatusCode)
                return result.Body.Items.Select(pod => pod.Name()).ToList();
            _logger.LogCritical("Error Getting Pods: {Reason}", result.Response.ReasonPhrase);
            throw new HttpRequestException();
        }

        public async Task<bool> DeletePodAsync(string podName)
        {
            try
            {
                var result = await _client.DeleteNamespacedPodWithHttpMessagesAsync(podName, _config.AgentNamespace);
                return result.Response.IsSuccessStatusCode;
            }
            catch (HttpOperationException e)
            {
                _logger.LogWarning(e, "Error Deleting {Pod}: {Response}", podName, e.Response.ReasonPhrase);
                return false;
            }
        }

        private async Task<AgentSpec> GetAgentSpecAsync(string ns)
        {
            var crdCollection = await ListAgentSpecAsync(ns);
            return crdCollection switch
            {
                null => throw new NoNullAllowedException(
                    "Crd Must exist in namespace prior to running this controller"),
                { Items: { } } when crdCollection.Items.Any() => crdCollection.Items.First().Spec,
                _ => default
            };
        }

        private async Task<CrdList> ListAgentSpecAsync(string ns)
        {
            try
            {
                var specResponse = await _client.ListNamespacedCustomObjectAsync(CrdConstants.Group,
                    CrdConstants.Version, ns, CrdConstants.CRD_AgentSpecPlural);
                if (specResponse is null)
                    throw new KubernetesException(
                        "CRD List Returned Null.  Make sure CRD Exists before running this controller");
                return JsonSerializer.Deserialize<CrdList>(specResponse.ToString()!,
                    HttpClientExtensions.CamelCaseOption);
            }
            catch (Exception ex)
            {
                // TODO: Handle Error
                _logger.LogError(ex, "{Msg}", ex.Message);
                return null;
            }
        }

        private async Task Initialize()
        {
            var ns = await _client.ListNamespaceWithHttpMessagesAsync();
            var test = ns.Body.Items.Where(x => x.Metadata.Name == _config.AgentNamespace);
            if (!test.Any())
            {
                var newNamespace = new V1Namespace
                {
                    Metadata = new V1ObjectMeta
                    {
                        Name = _config.AgentNamespace
                    }
                };
                var nsResult = await _client.CreateNamespaceWithHttpMessagesAsync(newNamespace);
                if (!nsResult.Response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error Creating Agent Namespace {nsResult.Response.ReasonPhrase}");
            }

            var secretResult = await _client.ListNamespacedSecretWithHttpMessagesAsync(_config.AgentNamespace);
            if (secretResult.Response.IsSuccessStatusCode &&
                secretResult.Body.Items.Any(x => x.Metadata.Name == "azdo-token"))
            {
                var delTokenResponse =
                    await _client.DeleteNamespacedSecretWithHttpMessagesAsync("azdo-token", _config.AgentNamespace);
                if (!delTokenResponse.Response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Error Deleting Token Secret: {delTokenResponse.Response.ReasonPhrase}");
                }
            }

            var secret = new V1Secret
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "azdo-token",
                    NamespaceProperty = _config.AgentNamespace
                },
                StringData = new Dictionary<string, string>()
            };
            secret.StringData.Add("pat", _adoConfig.Token);
            var result = await _client.CreateNamespacedSecretWithHttpMessagesAsync(secret, _config.AgentNamespace);
            if (!result.Response.IsSuccessStatusCode)
                _logger.LogError("KubeResponseError: {Reason}", result.Response.ReasonPhrase);
        }
    }
}