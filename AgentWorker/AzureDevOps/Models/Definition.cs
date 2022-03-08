using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class Definition
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }
    }
}