using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aca.Demo.Health;

public class GitHubHealthCheck : IHealthCheck
{
    
    private readonly IHttpClientFactory _httpClientFactory;
    
    public GitHubHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new ())
    {
        // Simple check. Can check for other things and also return 'Degraded' instead of
        // 'Healthy' or 'Unhealthy' where appropriate.
        try
        {
            var client = _httpClientFactory.CreateClient("GitHub");
            await client.GetAsync($"users/ChrisYoxall", cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}