module App.Rock
open System
open Generic.Utils

type RockState =
| Falling
| Terminated

type state ={
    RockState : RockState
    x : int 
    y : int 
    tick : int 
    redrawScreen : bool
}

let initialState = {
    RockState = Falling
    x = Console.BufferWidth / 2
    y = 0
    tick = 0
    redrawScreen = true 
}
let updateTick state =
    {state with tick = state.tick+1}
let updateRockPosition state  =
    {state with y = min (Console.BufferHeight-1) (state.y+1);redrawScreen = true}
    
let rockKeyboard k state =
    match k with 
    | ConsoleKey.Enter -> {state with y = 0}
    | ConsoleKey.Escape -> {state with RockState = Terminated}
    | _ -> state      

    
let redrawScreen state =
    if state.redrawScreen then 
        Console.Clear()
        displayMessage state.x state.y  ConsoleColor.Magenta "0"
        {state with redrawScreen = false}
    else 
        state

let redrawRock state =
    displayMessage state.x state.y  ConsoleColor.Magenta "0"
let pipeline = [|
    updateTick
    updateRockPosition
|]
let myLoop =
    createMainLoop 
        pipeline 
        (fun x -> x.RockState = Falling) 
        [|redrawRock|] 
        [|rockKeyboard|]
        (fun s -> s.redrawScreen)
        (fun s -> {s with redrawScreen = false})
    
let oldForeground = Console.ForegroundColor
Console.CursorVisible <- false

initialState
|> myLoop
|> ignore

Console.CursorVisible <- true
Console.ForegroundColor <- oldForeground
Console.Clear()