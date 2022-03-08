using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class MsVssBuildWebAgentJobsDataProvider
    {
        [JsonPropertyName("jobs")] public List<Job> Jobs { get; set; }
    }
}