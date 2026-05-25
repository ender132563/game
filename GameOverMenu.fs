module App.GameOverMenu
open System
open Generic.Menu 

type commands =
| NewGame 
| Exit 

let menu() =
    genericMenu
        20
        (Console.BufferHeight*3/4-1)
        [|  
            NewGame,"NewGame"
            Exit,"Exit"
        |]
        "⚰️"
        ConsoleColor.DarkRed
        Titles.gameOverTitle
        ConsoleColor.Red
        (20,15)