using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.RESTAPI.MicroService.Shared.Extensions ;

public static class WebApplicationExtensions
{
    public static void LogAppSettings ( this WebApplication app )
    {
        var logger = app.Services.GetService<Serilog.ILogger>();

        if ( logger == null )
            throw new ArgumentNullException ( nameof(app), "Unable to get logger" ) ;

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        logger.Information($"----- [Begin] '{environment}' Settings -----");
        logger.Information($"{(app.Configuration as IConfigurationRoot).GetDebugView()}");
        logger.Information($"----- [End] '{environment}' Settings -----");
    }
}