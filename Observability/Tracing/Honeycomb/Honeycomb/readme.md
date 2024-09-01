

### Project Configuration ###

The Honeycomb website at https://www.honeycomb.io/ has some decent documentation. Also refer to the documentation
on the OpenTelemetry website at https://opentelemetry.io/docs/languages/net/instrumentation/

Need to set the following environment variables. Note that these are set in the launchsettings.json
file currently for dev, and need to replace `HONEYCOMB_API_KEY`:

    export OTEL_SERVICE_NAME=my-service-name
    export OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf
    export OTEL_EXPORTER_OTLP_ENDPOINT="https://api.honeycomb.io"
    export OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=HONEYCOMB_API_KEY"

# General OpenTelemetry Notes #

Put these here for now....

### Azure ###

See Azure Monitor OpenTelemetry https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore


### Tracing in .NET ###

.NET is different from other languages/runtimes that support OpenTelemetry. The Tracing API is implemented by the System.Diagnostics API,
repurposing existing constructs like ActivitySource and Activity to be OpenTelemetry-compliant under the covers.


### About OpenTelemetry ###

Refer https://opentelemetry.io/docs/concepts/signals/traces/ for trace glossary.

Components making up the OpenTelemetry stack are:

- Application: The application/service to monitor.
- OpenTelemetry SDK: Provides APIs for generating telemetry data. Is integrated into the application.
- Instrumentation: Adds telemetry data collection to your code. Can be automatic (for supported libraries) or manual (using SDK APIs).
- Processor: Processes the generated telemetry data, potentially modifying or filtering it before export.
- Exporter: Sends the telemetry data to the chosen backend(s) service or to the OpenTelemetry Collector.
- OpenTelemetry Collector (optional): Receives, processes, and exports data to one or more backends. It can also perform data transformation and filtering.
- Backend Services: Observability platforms or storage systems where the telemetry data is ultimately sent.