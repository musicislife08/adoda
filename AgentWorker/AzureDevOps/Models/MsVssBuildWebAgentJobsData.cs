using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class MsVssBuildWebAgentJobsData
    {
        [JsonPropertyName("ms.vss-build-web.agent-jobs-data-provider")]
        public MsVssBuildWebAgentJobsDataProvider MsVssBuildWebAgentJobsDataProvider { get; set; }
    }
}