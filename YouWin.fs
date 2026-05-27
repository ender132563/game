module App.YouWin
open Generic.Menu
open System
open Generic.Utils
type command =
| Exit

let menu() = 
    genericMenu
        20
        (Console.BufferHeight*4/5-1)
        [|
            Exit,"Exit"
        |]
        "🚩"
        ConsoleColor.Magenta
        Titles.YouWinTitle
        ConsoleColor.DarkMagenta
        (Console.BufferWidth/2,Console.BufferHeight*1/4-1)