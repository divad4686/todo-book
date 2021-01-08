module Main

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Database

let connection =
    "User ID=postgres;Password=mypassword;Host=db;Port=5432;"

let fromJson<'T> request =
    request.request.rawForm |> Json.fromJson<'T>

[<EntryPoint>]
let main argv =

    let insertTodo request =

        let data = request |> fromJson<Todo>

        async {
            do! insertTodo connection data
            return! OK "TODO Inserted" request
        }

    let getTodoList request =
        async {
            let! todos = listTodo connection
            return! ok (todos |> Json.toJson) request
        }

    let app =
        choose [ GET
                 >=> choose [ path "/hello" >=> OK "Hello GET"
                              path "/todo"
                              >=> getTodoList
                              >=> Writers.setMimeType "application/json" ]
                 POST >=> choose [ path "/todo" >=> insertTodo ] ]


    let config =
        { defaultConfig with
              bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8080 ] }

    let server = startWebServer config app

    0 // return an integer exit code
