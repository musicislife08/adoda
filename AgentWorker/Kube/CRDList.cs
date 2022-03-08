using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.Kube
{
    public class CrdList
    {
        [JsonPropertyName("apiVersion")] public string ApiVersion { get; set; }

        [JsonPropertyName("items")] public List<Crd> Items { get; set; }

        [JsonPropertyName("kind")] public string Kind { get; set; }

        [JsonPropertyName("metadata")] public CrdMetadata Metadata { get; set; }
    }
}