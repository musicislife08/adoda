using System;
using System.Text.Json.Serialization;

namespace AgentWorker.Kube
{
    public class AgentSpec
    {
        [JsonPropertyName("image")] public string Image { get; set; }

        [JsonPropertyName("imagePullSecretName")]
        public string ImagePullSecretName { get; set; }

        [JsonPropertyName("prefix")] public string Prefix { get; set; }

        public string GeneratePodName()
        {
            Console.WriteLine($"GeneratePodName: {Prefix}");
            return $"{(string.IsNullOrWhiteSpace(Prefix) ? "agent" : Prefix)}-{DateTime.UtcNow.Ticks}";
        }

        public string GetImageName()
        {
            if (string.IsNullOrWhiteSpace(Image))
                throw new ArgumentNullException(Image, "Image cannot be blank");
            return Image;
        }
    }
}