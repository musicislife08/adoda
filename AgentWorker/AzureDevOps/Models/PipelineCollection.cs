using System.Collections.Generic;

namespace AgentWorker.AzureDevOps.Models
{
    public class PipelineCollection
    {
        public int Count { get; set; }
        public List<Pipeline> Value { get; set; }
    }
}