namespace AgentWorker.AzureDevOps.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool Enabled { get; set; }
        public string JobId { get; set; }
    }
}