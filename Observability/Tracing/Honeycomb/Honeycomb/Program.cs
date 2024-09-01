
using Honeycomb.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "dice-service";
const string serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

// Setup OpenTelemetry Tracing. This will create a Trace Provider, which is a factory for creating Tracers.
// Tracers create spans.
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion))
    .WithTracing(traceProviderBuilder =>
    {
        traceProviderBuilder
            .AddSource(serviceName, serviceVersion)
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter()
            .AddHttpClientInstrumentation()
            .SetSampler(new AlwaysOnSampler());
    });

// Add the Instrumentation service to make a common ActivitySource available for manual tracing.
builder.Services.AddSingleton<Instrumentation>(_ => new Instrumentation(serviceName, serviceVersion));

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Add middleware to the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();