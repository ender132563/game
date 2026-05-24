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
        (Console.BufferHeight*3/4-1)
        [|
            Continue,"Continue"
            SaveGame,"Save Game"
            Exit,"Exit"
        |]
        "👉"
        ConsoleColor.Cyan
        Titles.pauseTitle
        ConsoleColor.Red
        (20,15)
