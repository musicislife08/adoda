using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class Data
    {
        [JsonPropertyName("ParallelismTag")] public string ParallelismTag { get; set; }

        [JsonPropertyName("IsScheduledKey")] public string IsScheduledKey { get; set; }
    }
}