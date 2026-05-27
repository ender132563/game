module App.Pause
open System
open Generic.Menu 

type commands =
| Continue
| SaveGame
| Exit

let menu() =
    genericMenu
        20
        (Console.BufferHeight*4/5-1)
        [|
            Continue,"Continue"
            SaveGame,"Save Game"
            Exit,"Exit"
        |]
        "👉"
        ConsoleColor.Magenta
        Titles.pauseTitle
        ConsoleColor.Cyan
        (Console.BufferWidth/2,Console.BufferHeight*1/4-1)
