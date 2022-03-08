namespace AgentWorker.AzureDevOps
{
    public class AdoConfig
    {
        public string OrgUri { get; set; }
        public string Token { get; set; }
        public string AgentPool { get; set; }
        public int NumberOfWaitingAgents { get; set; }
    }
}