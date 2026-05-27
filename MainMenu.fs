module App.MainMenu
open System
open Generic.Menu
open Generic.Utils

type Commands = 
| NewGame
| LoadGame
| Exit


let showMenu()=
    
    genericMenu 
        20
        (Console.BufferHeight*4/5-1)
        [|
            NewGame,"New Game"
            LoadGame,"Load Game"
            Exit,"Exit"
        |] 
        "☠️"
        ConsoleColor.Blue
        Titles.mainTitle
        ConsoleColor.Green
        (Console.BufferWidth/2,Console.BufferHeight*1/4-1)