using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AgentWorker.AzureDevOps;
using AgentWorker.Kube;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace AgentWorker
{
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (host)
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddLogging();
                    services.Configure<AdoConfig>(x => hostContext.Configuration.GetSection("AdoConfig").Bind(x));
                    services.Configure<KubernetesConfig>(x =>
                        hostContext.Configuration.GetSection("KubernetesConfig").Bind(x));
                    services.AddHttpClient<IAzureDevOpsClient, AzureDevOpsClient>(client =>
                        {
                            var url = hostContext.Configuration["AdoConfig:OrgUri"];
                            var token = hostContext.Configuration["AdoConfig:Token"];
                            if (string.IsNullOrWhiteSpace(url))
                                throw new ArgumentNullException("OrgUrl", "Error: OrgUrl must be set");
                            if (string.IsNullOrWhiteSpace(url))
                                throw new ArgumentNullException("Token", "Error: Token must be set");
                            if (!url.EndsWith('/'))
                                url += '/';
                            client.BaseAddress = new Uri(url);
                            var credentials = Convert.ToBase64String(
                                Encoding.ASCII.GetBytes($":{token}"));
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Basic", credentials);
                        })
                        .SetHandlerLifetime(TimeSpan.FromMinutes(5));
                    services.AddTransient<IKubernetesService, KubernetesService>();
                    services.AddTransient<IAgentService, AgentService>();


                    services.AddHostedService<AgentWorker>();
                }).UseConsoleLifetime();
    }
}