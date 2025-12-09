using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace TikTok.Shared.Logging
{
    public static class LoggerConfiguration
    {
        public static ILogger CreateLogger(IConfiguration configuration, string applicationName)
        {
            return new Serilog.LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
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
                    retainedFileCountLimit: 30)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .CreateLogger();
        }

        public static ILogger CreateLogger(string applicationName, LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            return new Serilog.LoggerConfiguration()
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
                    retainedFileCountLimit: 30)
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .CreateLogger();
        }
    }
}
