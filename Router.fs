module App.Router
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

open App
type GameState =
| ShowingMenu
| StartGame
| Pause
| LoadGame
| SaveGame
| Terminated


    

let initialState = ShowingMenu


// let loadState = File.ReadAllText (Generic.Utils.path)
// let lastState = JsonSerializer.Deserialize (loadState)


let rec routerLoop state =
    match state with 
    | ShowingMenu -> 
        match MainMenu.showMenu() with 
        | MainMenu.NewGame -> StartGame
        | MainMenu.LoadGame -> LoadGame
        | MainMenu.Exit -> Terminated
        
    | StartGame -> 
        Game.mostrar()
        Pause
    | Pause -> 

        match Pause.menu() with 
        | Pause.Continue -> LoadGame
        | Pause.SaveGame -> SaveGame
        | Pause.Exit -> ShowingMenu
    | SaveGame  
    | LoadGame 
        // Game.reanudar lastState |> ignore
        // Pause
    | Terminated -> Terminated

    |> fun s -> 
        if s <> Terminated then 
            routerLoop s


let init () =
    
    routerLoop initialState