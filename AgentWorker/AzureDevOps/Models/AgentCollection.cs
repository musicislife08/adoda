using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class AgentCollection
    {
        [JsonPropertyName("count")] public int Count { get; set; }

        [JsonPropertyName("value")] public List<ReservedAgent> Value { get; set; }
    }
}