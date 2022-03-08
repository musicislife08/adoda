using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AgentWorker.AzureDevOps.Models;
using AgentWorker.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgentWorker.AzureDevOps
{
    public interface IAzureDevOpsClient
    {
        Task<List<Job>> ListJobRequestsAsync();
        Task<JobRequestCollection> GetJobsAsync();
        Task<ReservedAgent> GetAgentByNameAsync(string agentName);
        Task<AgentCollection> GetAgentsAsync();
        Task<bool> DeleteAgentAsync(int agentId);
    }

    public class AzureDevOpsClient : IAzureDevOpsClient
    {
        private readonly HttpClient _client;

        // TODO: Add Logging
        private readonly ILogger<AzureDevOpsClient> _logger;
        private readonly AdoConfig _config;
        private readonly int _poolId;

        public AzureDevOpsClient(ILogger<AzureDevOpsClient> logger, HttpClient client, IOptions<AdoConfig> options)
        {
            _client = client;
            _logger = logger;
            _config = options.Value;
            _poolId = GetPoolIdAsync().GetAwaiter().GetResult();
        }

        private async Task<int> GetPoolIdAsync()
        {
            _logger.LogDebug("Get Pool Id Async");
            var msg = new HttpRequestMessage(HttpMethod.Get,
                $"_apis/distributedtask/pools?poolName={_config.AgentPool}&api-version=6.1-preview.1");
            var response = await _client.SendAsync(msg);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<PoolCollection>();
            if (content is null)
                throw new NullReferenceException($"Error Getting Pools: {response.ReasonPhrase}");
            if (content.Value is null)
                throw new NullReferenceException($"Error Getting Pool Value: {response.ReasonPhrase}");
            if (!content.Value.Any())
                throw new NullReferenceException($"No Pools with the name {_config.AgentPool}");
            return content.Value.First().Id;
        }

        public async Task<ReservedAgent> GetAgentByNameAsync(string agentName)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get,
                $"_apis/distributedtask/pools/{_poolId}/agents?agentName={agentName}&api-version=6.1-preview.1");
            var response = await _client.SendAsync(msg);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<AgentCollection>();
            return content?.Value.FirstOrDefault();
        }

        public async Task<AgentCollection> GetAgentsAsync()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get,
                $"_apis/distributedtask/pools/{_poolId}/agents?api-version=6.1-preview.1");
            var response = await _client.SendAsync(msg);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            //var rawContent = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadFromJsonAsync<AgentCollection>();
            return content;
        }

        public async Task<JobRequestCollection> GetJobsAsync()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get,
                $"_apis/distributedtask/pools/{_poolId}/jobrequests?api-version=6.1-preview.1");
            var response = await _client.SendAsync(msg);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            //var rawContent = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadFromJsonAsync<JobRequestCollection>();
            return content;
        }

        public async Task<List<Job>> ListJobRequestsAsync()
        {
            var option = new JsonSerializerOptions() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            option.Converters.Add(new UnixEpochDateConverter());
            option.Converters.Add(new JsonStringEnumConverter());
            var msg = new HttpRequestMessage(HttpMethod.Get,
                $"_settings/agentpools?poolId={_poolId}&__rt=fps&__ver=2");
            var response = await _client.SendAsync(msg);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<UndocumentedJobs>(option);
            return content?.Fps.DataProviders.Data.MsVssBuildWebAgentJobsDataProvider.Jobs;
        }

        public async Task<bool> DeleteAgentAsync(int agentId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Delete,
                $"_apis/distributedtask/pools/{_poolId}/agents/{agentId}?api-version=6.1-preview.1");
            var result = await _client.SendAsync(msg);
            return result.IsSuccessStatusCode;
        }
    }
}