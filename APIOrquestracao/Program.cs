using APIOrquestracao.Clients;
using APIOrquestracao.Tracing;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Documentacao do OpenTelemetry:
// https://opentelemetry.io/docs/instrumentation/net/getting-started/

// Integracao do OpenTelemetry com Jaeger em .NET:
// https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/docs/trace/getting-started-jaeger#collect-and-visualize-traces-using-jaeger

// Documentacaoo do Jaeger:
// https://www.jaegertracing.io/docs/1.56/

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ContagemClient>();

builder.Services.AddOpenTelemetry().WithTracing(traceProvider =>
{
    traceProvider
        .AddSource(OpenTelemetryExtensions.ServiceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                    serviceVersion: OpenTelemetryExtensions.ServiceVersion))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter()
        .AddConsoleExporter();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
