using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;
using Idasen.RESTAPI.MicroService.Shared.Settings ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;
using Microsoft.Extensions.Logging ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.Extensions ;

public static class WebApplicationBuilderExtensions
{
    public static void AddMicroServiceShared ( this WebApplicationBuilder builder )
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.Configuration
                //.SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json",
                            optional: false,
                            reloadOnChange: true)
               .AddJsonFile($"appsettings.{environment}.json",
                            optional: true)
               .AddEnvironmentVariables();

        // Add logging.
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context,
                                 services,
                                 configuration) => configuration.ReadFrom.Configuration(context.Configuration)
                                                                .ReadFrom.Services(services)
                                                                .Enrich.FromLogContext()
                                                                .WriteTo.Console());

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient < IMicroServiceSettingsProvider , MicroServiceSettingsProvider > ( ) ;
        builder.Services.AddTransient < ISettingsCaller , SettingsCaller > ( ) ;
        builder.Services.AddTransient < IMicroServiceCaller , MicroServiceCaller > ( ) ;
        builder.Services.AddTransient < IRequestForwarder , RequestForwarder > ( ) ;
        builder.Services.AddTransient < IMicroServiceSettingsDictionary , MicroServiceSettingsDictionary > ( ) ;
        builder.Services.AddTransient < IMicroServiceSettingsUriCreator , MicroServiceSettingsUriCreator > ( ) ;
        builder.Services.AddTransient ( _ => builder.Configuration
                                                    .GetSection ( nameof ( MicroServicesSettings ) )
                                                    .Get < IList < MicroServiceSettings > > ( ) ) ;

        // see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        builder.Services.AddHostedService ( serviceProvider =>
#pragma warning disable CS8604
                                                new StartupBackgroundService ( serviceProvider.GetService < Serilog.ILogger > ( ) ,
                                                                               serviceProvider.GetService < IMicroServiceSettingsProvider > ( ) ,
                                                                               serviceProvider.GetService < IMicroServiceSettingsUriCreator > ( ),
                                                                               serviceProvider.GetService < StartupHealthCheck > ( ) ) ) ;
#pragma warning restore CS8604
        builder.Services.AddSingleton < StartupHealthCheck > ( ) ;
        builder.Services.AddHealthChecks ( )
               .AddCheck < StartupHealthCheck > ( "Startup" ,
                                                  tags : new [ ] { "ready" } ) ;
    }
}