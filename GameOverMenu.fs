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
// let updateRock state =
//     if state.Rocks <> [] then 
//         state.Rocks
//         |> Seq.map (fun rock -> {rock with y=rock.y+1})
//         |> Seq.filter (fun rock -> rock.y  < Console.BufferWidth-2)
//         |> Seq.toList
//         |> fun newRocks ->
//             {state with Rocks = newRocks}  
//     else state
// let agregarMisilenemigo state =
//     let newRock = {
//         x = random.Next(maxX)
//         y = 0
//     } 
//     {state with  Rocks = newRock :: state.Rocks}

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

    
// let rec rain state = 
//     async {
//         let newState =
//             state
//             |> updateRock
//             |>

//         do! Async.Sleep 200

//         return! rain newState
//     }

// rock()|> Async.StartImmediate