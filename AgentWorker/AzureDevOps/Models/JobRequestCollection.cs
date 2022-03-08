using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class JobRequestCollection
    {
        [JsonPropertyName("count")] public int Count { get; set; }

        [JsonPropertyName("value")] public List<JobRequest> Value { get; set; }
    }
}