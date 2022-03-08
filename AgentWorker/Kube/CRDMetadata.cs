using System;
using System.Text.Json.Serialization;

namespace AgentWorker.Kube
{
    public class CrdMetadata
    {
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; }

        [JsonPropertyName("generation")] public int Generation { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("namespace")] public string Namespace { get; set; }

        [JsonPropertyName("resourceVersion")] public string ResourceVersion { get; set; }

        [JsonPropertyName("selfLink")] public string SelfLink { get; set; }

        [JsonPropertyName("uid")] public string Uid { get; set; }

        [JsonPropertyName("continue")] public string Continue { get; set; }
    }
}