namespace GuildManager.Backend

module HelloController =
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Giraffe
    open GuildManager.Backend.Dto

    let handleGetHello =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe"
                }
                return! json response next ctx
            }
