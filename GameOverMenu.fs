module App.GameOverMenu
open System
open Generic.Utils
open Generic.Menu


type commands =
| NewGame 
| Exit 

// type rock = {
//     x : int 
//     y : int
// }

// type state = {
//     Rocks : rock list
// }
// let initialState = {
//     Rocks = []
// }
// let updateRock state =
//     state.Rocks
//     |> Seq.map (fun rock -> {rock with y=rock.y+1})
//     |> Seq.filter (fun rock -> rock.y  < Console.BufferHeight-2)
//     |> Seq.toList
//     |> fun newRocks ->
//         {state with Rocks = newRocks}  
// let agregarMisilenemigo state =
//     let newRock = {
//         x = random.Next maxX
//         y = 0
//     } 
//     {state with  Rocks = newRock :: state.Rocks}

// let rec rain state = 
//     async {
//         do! Async.Sleep 25
//         let newState =

//             state
//             |> agregarMisilenemigo
//             |> updateRock
//             |> fun state -> 
//                 state.Rocks 
//                 |> List.iter (fun rock -> displayMessage rock.x rock.y ConsoleColor.Blue "🩸")
//                 state
//         return! rain newState
//     }
let menu() =
    
    // rain initialState |> Async.StartImmediate
    let manu =
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
        
    manu
    
