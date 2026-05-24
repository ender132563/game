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
        (Console.BufferHeight*3/4-1)
        [|
            NewGame,"New Game"
            LoadGame,"Load Game"
            Exit,"Exit"
        |] 
        "☠️"
        ConsoleColor.Red
        Titles.mainTitle
        ConsoleColor.Green
        (15,15)