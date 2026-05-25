module Generic.Utils 
open System.Threading
open System

let createMainLoop pipeline isProgramRunning  redrawPipeline keypipeline needToRedraw clearRedraw =
    let processKeyboard (state:'state)=
        if Console.KeyAvailable then 
            let k = Console.ReadKey true 
            keypipeline
            |> Array.fold ( fun acc f -> f k.Key acc) state
        else state
    let redrawScreen (state:'state)=
        if needToRedraw state then 
            Console.Clear()
            redrawPipeline
            |> Array.iter (fun f -> f state)
            clearRedraw state
        else state

    let rec mainLoop (state:'state)=
        pipeline
        |> Array.fold (fun acc f -> f acc) state
        |> redrawScreen
        |> processKeyboard
        |> fun newState -> 
            if isProgramRunning newState then 
                Thread.Sleep 25
                mainLoop newState
            else 
                newState
    mainLoop
        
let displayMessage x y color (msg:string) =
    Console.SetCursorPosition (x,y)
    Console.ForegroundColor <- color 
    Console.Write msg 
let displayMessageRight y color (msg:String) =
    let x = Console.BufferWidth - msg.Length 
    displayMessage x y color msg 
    
let random = Random()
let cache = "playerProgress.json"
let savingPath = "savedProgress.json"
let maxY = Console.BufferHeight-1
let maxX = Console.BufferWidth-1