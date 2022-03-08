using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class AgentSpecification
    {
        [JsonPropertyName("VMImage")]
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Global
        public string VMImage { get; set; }

        [JsonPropertyName("vmImage")]
        // ReSharper disable once UnusedMember.Global
        public string VmImage { get; set; }
    }
}