using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TikTok.Shared.Logging
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string applicationName)
        {
            var logger = LoggerConfiguration.CreateLogger(builder.Configuration, applicationName);
            
            builder.Host.UseSerilog(logger);
            
            return builder;
        }

        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder, string applicationName)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", applicationName)
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Application} {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: $"logs/{applicationName}-.log",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Application} {Message:lj}{NewLine}{Exception}",
                        retainedFileCountLimit: 30);
            });

            return hostBuilder;
        }

        public static IApplicationBuilder UseSerilogRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                };
            });

            return app;
        }
    }
}
