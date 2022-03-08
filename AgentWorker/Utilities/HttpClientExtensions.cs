using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgentWorker.Utilities
{
    public static class HttpClientExtensions
    {
        public static readonly JsonSerializerOptions CamelCaseOption = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static async Task<TPayload> ReadContentAsync<TPayload>(this HttpContent content,
            JsonSerializerOptions options = null)
        {
            options ??= CamelCaseOption;
            var contentString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TPayload>(contentString, options);
        }
    }
}