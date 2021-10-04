module TodoGiraffe.WebApp
open Microsoft.AspNetCore.Http
open Giraffe

let setMetricsRoute: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let route = ctx.Request.Path
        ctx.AddMetricsCurrentRouteName(route.ToString())
        next ctx
let webApp db =
    setMetricsRoute
    >=> choose [ route "/" >=> text "Hello"
                 setStatusCode 404 >=> text "Not Found" ]