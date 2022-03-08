namespace AgentWorker.Kube
{
    public class KubernetesConfig
    {
        public string AgentNamespace { get; set; }
        public int AgentMinCount { get; set; }
        public int AgentMaxCount { get; set; }
        public string LocalKubeconfigPath { get; set; }
        public string KubeconfigContextName { get; set; }
    }
}