using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class ReservedAgent
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("version")] public string Version { get; set; }

        [JsonPropertyName("osDescription")] public string OsDescription { get; set; }

        [JsonPropertyName("enabled")] public bool Enabled { get; set; }

        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("provisioningState")]
        public string ProvisioningState { get; set; }

        [JsonPropertyName("accessPoint")] public string AccessPoint { get; set; }
    }
}