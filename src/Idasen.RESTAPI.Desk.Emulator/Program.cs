using System.Reactive.Concurrency ;
using Idasen.RESTAPI.Desk.Emulator.Desks ;
using Idasen.RESTAPI.Desk.Emulator.Idasen ;
using Idasen.RESTAPI.Desk.Emulator.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.Extensions ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks ;
using Serilog ;

var builder = WebApplication.CreateBuilder ( args ) ;

// Add services to the container.

builder.Services.AddControllers ( ) ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ( ) ;
builder.Services.AddSwaggerGen ( ) ;
builder.Services.AddSingleton < IRestDesk , RestDesk > ( ) ;
builder.Services.AddSingleton < IFakeDesk , FakeDesk > ( ) ;
builder.Services.AddSingleton < IScheduler > ( TaskPoolScheduler.Default ) ;
builder.Services.AddHealthChecks();

// todo begin this is the same as code in shared
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
// end

var app = builder.Build ( ) ;

app.LogAppSettings();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/healthz/ready",
                    new HealthCheckOptions
                    {
                        Predicate = healthCheck => healthCheck.Tags.Contains("ready")
                    });
app.MapHealthChecks("/healthz/live",
                    new HealthCheckOptions
                    {
                        Predicate = _ => false
                    });

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwagger ( ) ;
    app.UseSwaggerUI ( ) ;
}

// app.UseHttpsRedirection ( ) ;

app.UseAuthorization ( ) ;

app.MapControllers ( ) ;

app.Run ( ) ;