using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace AgentWorker.Kube
{
    public class Templates
    {
        private const string _podTemplate = "PodTemplate.k8s";

        public static async Task<V1Pod> ReadPodTemplateAsync()
        {
            await using var stream =
                typeof(Templates).Assembly.GetManifestResourceStream($"{typeof(Templates).Namespace}.{_podTemplate}");
            return await Yaml.LoadFromStreamAsync<V1Pod>(stream);
        }
    }
}