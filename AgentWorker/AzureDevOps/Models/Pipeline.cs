namespace AgentWorker.AzureDevOps.Models
{
    public class Pipeline
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }
}