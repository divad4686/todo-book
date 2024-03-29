module TodoGiraffe.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Linq
open Giraffe
open FSharp.Control.Tasks
open Serilog
open App.Metrics.AspNetCore
open App.Metrics
open App.Metrics.Formatters.Prometheus
open TodoGiraffe.WebApp

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: Microsoft.Extensions.Logging.ILogger) =
    Log.Error(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
    |> ignore

let configureApp webApp (app: IApplicationBuilder) =
    let env =
        app.ApplicationServices.GetService<IWebHostEnvironment>()

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler(errorHandler))
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore


[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")

    Log.Logger <-
        LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger()

    let db =
        match Environment.GetEnvironmentVariable("DBConnectionString") with
        | x when x |> String.IsNullOrEmpty -> "User ID=postgres;Password=mypassword;Host=localhost;Port=5432;"
        | x -> x

    let webApp = webApp db
    let configApp = configureApp webApp

    let metrics =
        AppMetrics
            .CreateDefaultBuilder()
            .OutputMetrics.AsPrometheusPlainText()
            .OutputMetrics.AsPrometheusProtobuf()
            .Build()
    
    let configPrometheus (options: MetricsWebHostOptions) =
        options.EndpointOptions <-
            fun endOptions ->
                endOptions.MetricsEndpointOutputFormatter <-
                    metrics
                        .OutputMetricsFormatters
                        .OfType<MetricsPrometheusTextOutputFormatter>()
                        .First()

    Host
        .CreateDefaultBuilder(args)
        .ConfigureMetrics(metrics)
        .UseMetrics(configPrometheus)
        .UseSerilog()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .Configure(Action<IApplicationBuilder> configApp)
                .ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    0
