module App.MainMenu
open System
open Generic.Menu

type Commands = 
| NewGame
| LoadGame
| Exit

let showMenu()=
    genericMenu 
        (Console.BufferWidth/2-1)
        (Console.BufferHeight/2-1)
        [|
            NewGame,"New Game"
            LoadGame,"Load Game"
            Exit,"Exit"
        |] 
        "🥵"

