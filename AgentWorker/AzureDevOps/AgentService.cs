using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentWorker.AzureDevOps.Models;
using Microsoft.Extensions.Logging;

namespace AgentWorker.AzureDevOps
{
    public interface IAgentService
    {
        Task<List<Agent>> GetAgentsAsync();
        Task<List<JobRequest>> GetJobsAsync();
        Task RemoveAgentAsync(int id);
    }

    public class AgentService : IAgentService
    {
        private readonly ILogger<AgentService> _logger;
        private readonly IAzureDevOpsClient _client;

        public AgentService(ILogger<AgentService> logger, IAzureDevOpsClient client)
        {
            _logger = logger;
            _logger.LogInformation("Initializing");
            _client = client;
        }

        public async Task<List<Agent>> GetAgentsAsync()
        {
            _logger.LogDebug("Getting Agents Async");
            var adoAgents = await _client.GetAgentsAsync();
            return adoAgents.Value.Select(agent => new Agent()
                {Id = agent.Id, Name = agent.Name, Enabled = agent.Enabled, Status = agent.Status}).ToList();
        }

        public async Task<List<JobRequest>> GetJobsAsync()
        {
            var result = await _client.GetJobsAsync();
            return result.Value;
        }

        public async Task RemoveAgentAsync(int id)
        {
            await _client.DeleteAgentAsync(id);
        }
    }
}