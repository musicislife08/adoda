using System.Collections.Generic;

namespace AgentWorker.AzureDevOps.Models
{
    public class PoolCollection
    {
        public int Count { get; set; }
        public List<Pool> Value { get; set; }
    }
}