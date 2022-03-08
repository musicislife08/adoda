using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class DataProviders
    {
        [JsonPropertyName("data")] public MsVssBuildWebAgentJobsData Data { get; set; }
    }
}