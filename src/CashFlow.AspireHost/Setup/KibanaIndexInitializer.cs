using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CashFlow.AppHost.Setup
{
    public class KibanaIndexInitializer : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KibanaIndexInitializer> _logger;

        public KibanaIndexInitializer(IHttpClientFactory httpClientFactory, ILogger<KibanaIndexInitializer> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = _httpClientFactory.CreateClient("kibana");

            for (int attempt = 1; attempt <= 10; attempt++)
            {
                try
                {
                    _logger.LogInformation("Checking if Kibana is up... (attempt {Attempt} | LogType={LogType}", attempt, "application");
                    var response = await client.GetAsync("/api/data_views", stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync(stoppingToken);
                        if (content.Contains("\"name\":\"logs\""))
                        {
                            _logger.LogInformation("Kibana index 'logs' already exists. | LogType={LogType}", "application");
                            return;
                        }

                        _logger.LogInformation("Creating Kibana index 'logs'... | LogType={LogType}", "application");

                        var payload = new
                        {
                            data_view = new
                            {
                                title = "logs",
                                name = "logs"
                            }
                        };

                        var postResponse = await client.PostAsJsonAsync("/api/data_views/data_view", payload, stoppingToken);
                        postResponse.EnsureSuccessStatusCode();

                        _logger.LogInformation("Kibana index 'logs' created successfully. | LogType={LogType}", "application");
                        return;
                    }
                }
                catch
                {
                    _logger.LogInformation( "Kibana not ready, retrying... | LogType={LogType}", "application");
                }

                await Task.Delay(20000, stoppingToken);
            }

            _logger.LogError("Kibana was not available after multiple attempts. Skipping setup. | LogType={LogType}", "application");
        }
    }
}