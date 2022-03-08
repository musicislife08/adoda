using System;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class Pool
    {
        [JsonPropertyName("createdOn")] public DateTime CreatedOn { get; set; }

        [JsonPropertyName("autoProvision")] public bool AutoProvision { get; set; }

        [JsonPropertyName("autoUpdate")] public bool AutoUpdate { get; set; }

        [JsonPropertyName("autoSize")] public bool AutoSize { get; set; }

        [JsonPropertyName("targetSize")] public int? TargetSize { get; set; }

        [JsonPropertyName("agentCloudId")] public int? AgentCloudId { get; set; }

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("scope")] public string Scope { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("isHosted")] public bool IsHosted { get; set; }

        [JsonPropertyName("poolType")] public string PoolType { get; set; }

        [JsonPropertyName("size")] public int Size { get; set; }

        [JsonPropertyName("isLegacy")] public bool IsLegacy { get; set; }

        [JsonPropertyName("options")] public string Options { get; set; }

        public override string ToString()
        {
            return $"{Name} - {PoolType}; Hosted={this.IsHosted}; Agent Cloud ID: {AgentCloudId}";
        }
    }
}