using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class UndocumentedJobs
    {
        [JsonPropertyName("fps")] public Fps Fps { get; set; }
    }
}