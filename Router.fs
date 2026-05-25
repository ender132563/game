module App.Router
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open Generic.Utils
open App
type GameState =
| ShowingMenu
| StartGame
| Pause
| LoadGame
| ContinueGame
| SaveGame
| Terminated
| GameOver

    

let initialState = ShowingMenu




let rec routerLoop state =

    let jsonString = File.ReadAllText cache
    let lastState = JsonSerializer.Deserialize<Game.State>(jsonString,Game.options)
    

    match state with 
    | ShowingMenu -> 
        match MainMenu.showMenu() with 
        | MainMenu.NewGame -> StartGame
        | MainMenu.LoadGame -> LoadGame
        | MainMenu.Exit -> Terminated
        
    | StartGame -> 
        match Game.mostrar Game.InitialState with 
        | Game.GameOver -> GameOver
        | _ -> Pause
    | Pause -> 

        match Pause.menu() with 
        | Pause.Continue -> ContinueGame
        | Pause.SaveGame -> SaveGame
        | Pause.Exit -> ShowingMenu

    |ContinueGame ->

        match Game.mostrar lastState with
        | Game.GameOver -> GameOver
        | _ -> Pause
           
    | SaveGame ->
        File.WriteAllText (savingPath,jsonString)
        Pause
    | LoadGame ->
        let jsonSavedGameString = File.ReadAllText savingPath
        let savedState = JsonSerializer.Deserialize<Game.State>(jsonSavedGameString,Game.options)
        match Game.mostrar savedState with 
        | Game.GameOver -> GameOver
        | _ -> Pause

    | Terminated -> Terminated
    | GameOver -> 
        match GameOverMenu.menu() with
        | GameOverMenu.NewGame -> StartGame
        | GameOverMenu.Exit -> Terminated

    |> fun s -> 
        if s <> Terminated then 
            routerLoop s


let init () =
    
    routerLoop initialState