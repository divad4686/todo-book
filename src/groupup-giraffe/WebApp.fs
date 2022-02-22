module GroupUp.WebApp
open Microsoft.AspNetCore.Http
open Giraffe
open GroupUp.Domain
open FSharp.Control.Tasks
open Serilog

let setMetricsRoute: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let route = ctx.Request.Path
        ctx.AddMetricsCurrentRouteName(route.ToString())
        next ctx

let createGroupUp : HttpHandler =
    handleContext(
        fun ctx ->
            task {
                Log.Logger.Information("testing")
                return! ctx.WriteTextAsync "Done working"
            })

let webApp db =
    setMetricsRoute
    >=> choose [ route "/" >=> text "Hello World"
                 route "/groupup" >=> choose [
                     route "/create" >=> POST >=> createGroupUp
                 ]
                 setStatusCode 404 >=> text "Not Found"
                 ]