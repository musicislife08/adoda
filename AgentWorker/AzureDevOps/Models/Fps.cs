using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class Fps
    {
        [JsonPropertyName("dataProviders")] public DataProviders DataProviders { get; set; }
    }
}