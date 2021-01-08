module Database

open Npgsql
open Dapper
open System.Runtime.Serialization

let inline (=>) a b = a, box b

[<CLIMutable>]
[<DataContract>]
type Todo =
    { [<field: DataMember(Name = "id")>]
      Id: string
      [<field: DataMember(Name = "title")>]
      Title: string
      [<field: DataMember(Name = "text")>]
      Text: string
      [<field: DataMember(Name = "url")>]
      Url: string
      [<field: DataMember(Name = "completed")>]
      Completed: bool }

let insertTodo connStr data: Async<unit> =
    let sql = """
    INSERT into todo.todo(id,title,text,url,completed)
    values(@id,@title,@text,@url,@completed)
    """

    async {
        use conn = new NpgsqlConnection(connStr)
        do! conn.ExecuteAsync(sql, data)
    }

let listTodo connStr: Async<Todo seq> =
    let sql = "SELECT * from todo.todo"

    async {
        use conn = new NpgsqlConnection(connStr)
        let! result = conn.QueryAsync<Todo>(sql)
        return result
    }
