module GuildManager.Backend.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open GuildManager.Backend.HelloController

let webApp =
    choose [
        subRoute "/api"
            (choose [
                GET >=> choose [
                    route "/hello" >=> handleGetHello
                ]
            ])
        setStatusCode 404 >=> text "Not Found" ]

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configCors (builder: CorsPolicyBuilder) = 
    builder
        .WithOrigins("https://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

let configApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true ->
        app.UseDeveloperExceptionPage()
    | false ->
        app
            .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configCors)
        .UseGiraffe(webApp)

let configServices (services: IServiceCollection) =
    services.AddCors()      |> ignore
    services.AddGiraffe()   |> ignore

let configLogging (builder: ILoggingBuilder) =
    builder
        .AddConsole()
        .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(Action<IApplicationBuilder> configApp)
                    .ConfigureServices(configServices)
                    .ConfigureLogging(configLogging)
                    |> ignore)
        .Build()
        .Run()
    0
