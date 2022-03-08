using System.Text.Json.Serialization;

namespace AgentWorker.Kube
{
    public record Crd
    {
        [JsonPropertyName("apiVersion")] public string ApiVersion { get; set; }

        [JsonPropertyName("kind")] public string Kind { get; set; }

        [JsonPropertyName("metadata")] public CrdMetadata Metadata { get; set; }

        [JsonPropertyName("spec")] public AgentSpec Spec { get; set; }
    }
}