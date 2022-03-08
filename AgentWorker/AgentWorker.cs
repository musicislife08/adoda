using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgentWorker.AzureDevOps;
using AgentWorker.AzureDevOps.Models;
using AgentWorker.Kube;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgentWorker
{
    public class AgentWorker : BackgroundService
    {
        private readonly ILogger<AgentWorker> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IAgentService _agentService;
        private readonly IKubernetesService _kubernetesService;
        private readonly AdoConfig _adoConfig;
        private const int WaitMs = 15000;

        public AgentWorker(
            ILogger<AgentWorker> logger,
            TelemetryClient telemetryClient,
            IAgentService agentService,
            IKubernetesService kubernetesService,
            IOptions<AdoConfig> options)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _agentService = agentService;
            _kubernetesService = kubernetesService;
            _adoConfig = options.Value;
            _logger.LogDebug("{Name} Initialized", nameof(AgentWorker));
        }

        private async Task<List<Agent>> CleanupOfflineAgents(IEnumerable<Agent> agents)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(CleanupOfflineAgents)))
            {
                foreach (var agent in agents.Where(x => x.Status == "offline" || x.Enabled == false))
                {
                    await _agentService.RemoveAgentAsync(agent.Id);
                    await _kubernetesService.DeletePodAsync(agent.Name);
                }
                return await _agentService.GetAgentsAsync();
            }
        }

        private async Task<List<string>> GetDanglingPodsAsync(IList<Agent> agents)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(GetDanglingPodsAsync)))
            {
                var pods = await _kubernetesService.GetPodsAsync();
                return pods.Where(pod => agents.All(agent => agent.Name != pod)).ToList();
            }
        }

        private async Task CleanupDanglingPods(IList<Agent> agents)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(CleanupDanglingPods)))
            {
                try
                {
                    var pods = await GetDanglingPodsAsync(agents);
                    foreach (var pod in pods)
                    {
                        await _kubernetesService.DeletePodAsync(pod);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private async Task RemoveAgents(List<Agent> agents)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(RemoveAgents)))
            {
                foreach (var agent in agents)
                {
                    await _agentService.RemoveAgentAsync(agent.Id);
                    await _kubernetesService.DeletePodAsync(agent.Name);
                }
            }
        }

        private async Task WaitForAgentsToCreate(IReadOnlyCollection<string> agents)
        {
            using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(WaitForAgentsToCreate)))
            {
                var allAgentsCreated = false;
                do
                {
                    var adoAgents = await _agentService.GetAgentsAsync();
                    var createdAgents = agents
                        .Select(agent => adoAgents.Find(x => x.Status != "offline" & x.Name == agent))
                        .Where(result => result is not null).ToList();
                    if (createdAgents.Count == agents.Count)
                        allAgentsCreated = true;
                    await Task.Delay(5000);
                } while (!allAgentsCreated);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Agent Worker running at: {Time}", DateTimeOffset.UtcNow);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>(nameof(ExecuteAsync)))
                {
                    var adoAgents = await _agentService.GetAgentsAsync();
                    adoAgents = await CleanupOfflineAgents(adoAgents);
                    await CleanupDanglingPods(adoAgents);
                    var jobs = await _agentService.GetJobsAsync();
                    var activeJobs = jobs.Where(job => job.ReservedAgent != null).ToList();
                    var waitingJobs = jobs.Where(job => job.ReservedAgent == null).ToList();
                    var neededAgentCount = activeJobs.Count + waitingJobs.Count + _adoConfig.NumberOfWaitingAgents;
                    if (adoAgents.Count == neededAgentCount)
                    {
                        await Task.Delay(WaitMs, stoppingToken);
                        continue;
                    }
                    if (adoAgents.Count > neededAgentCount)
                    {
                        foreach (var job in activeJobs)
                        {
                            adoAgents.RemoveAll(agent => agent.Name == job.ReservedAgent.Name);
                        }
                        if (adoAgents.Count > neededAgentCount)
                            adoAgents.RemoveRange(0, adoAgents.Count - neededAgentCount);
                        await RemoveAgents(adoAgents);
                        await Task.Delay(WaitMs, stoppingToken);
                        continue;
                    }
                    var newAgents = new List<string>();
                    for (var i = 0; i < neededAgentCount - adoAgents.Count; i++)
                    {
                        var agent = await _kubernetesService.CreateAgentAsync();
                        newAgents.Add(agent);
                    }
                    await WaitForAgentsToCreate(newAgents);
                }
            }
        }
    }
}