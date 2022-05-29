using Idasen.RESTAPI.MicroService.Shared.Extensions ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.Settings ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks ;

var builder = WebApplication.CreateBuilder ( args ) ;

builder.AddMicroServiceShared ( ) ;

var app = builder.Build ( ) ;

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwagger ( ) ;
    app.UseSwaggerUI ( ) ;
}

var test = builder.Configuration
                  .GetSection ( nameof ( MicroServicesSettings ) )
                  .Get < IList < MicroServiceSettings > > ( ) ;

app.UseHttpsRedirection ( ) ;
app.MapHealthChecks ( "/healthz" ) ;
app.MapHealthChecks ( "/healthz/ready" ,
                      new HealthCheckOptions
                      {
                          Predicate = healthCheck => healthCheck.Tags.Contains ( "ready" )
                      } ) ;
app.MapHealthChecks ( "/healthz/live" ,
                      new HealthCheckOptions
                      {
                          Predicate = _ => false
                      } ) ;

app.MapGet ( "/desk/" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService < IRequestForwarder > ( )!
                          .Forward ( httpContext ) ;
             } )
   .WithName ( "GetDesk" ) ;

app.MapGet ( "/desk/height/" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService < IRequestForwarder > ( )!
                          .Forward ( httpContext ) ;
             } )
   .WithName ( "GetDeskHeight" ) ;

app.MapGet ( "/settings" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService < ISettingsResponseCreator > ( )!
                          .Response ( app.Services ,
                                      httpContext ) ;
             } )
   .WithName ( "GetSettings" ) ;

app.Run ( ) ;