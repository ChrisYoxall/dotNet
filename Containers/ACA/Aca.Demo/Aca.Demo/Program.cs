using Aca.Demo.Health;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Register HttpClientFactory and named GitHub client.
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "DemoApp");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
});

builder.Services.AddOpenApi();

// Don't need the 'AddCheck' here, can just do 'builder.Services.AddHealthChecks()', which will
// return 'Healthy' if the current service is running. However, if your service has dependencies
// you can add a check on those also by adding a 'AddCheck' for each dependency. Returning
// a 'degraded' here as it assumes there is some functionality that is still working. Depends on the 
// 'GitHubHealthCheck' being written, which must implement IHealthCheck.
// Note that the degraded get overwritten by adding the response writer from third party nuget below. 
// Note also that the same project provides health checks for many common services so you may not
// need to write any custom health check. Refer https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
builder.Services.AddHealthChecks()
    .AddCheck<GitHubHealthCheck>("GitHub", HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

// Don't need to specify any options if you want a simple good/bad response. Can be nice to get more
// information which can be achieved by writing your own ResponseWriter. There are already some packages
// written by others that can be used, for example install the AspNetCore.HealthChecks.UI.Client" nuget package. The
// documentation is at https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks   The UI.Client includes
// the response writer, while the full UI is in the UI package.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
