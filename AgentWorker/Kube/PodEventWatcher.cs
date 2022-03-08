using k8s;
using k8s.Models;

namespace AgentWorker.Kube
{
    public static class PodEventWatcher
    {
        public static bool IsInSuccededOrFailedPhase(this V1Pod pod, WatchEventType eventType)
        {
            return eventType == WatchEventType.Modified && pod.IsInSuccededOrFailedPhase();
        }
    }
}