module App.Game
open System
open Generic.Utils
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
let path = "playerProgress.json"

type ProgramState = 
| Running
| Terminated

type AlienState =
| Active
| Dead 

type State = {
    programState : ProgramState
    AlienState : AlienState
    AlienY : int 
    BulletX : int 
    BulletY : int 
    PlayerX : int 
    PlayerY : int 
    PlayerBulletPosition : int
    triggerBullet : bool
    redrawBullet : bool
    redrawScreen : bool
    tick : int
    clock : int 

}
// alienY playerY 
let InitialState = {
    programState = Running
    AlienState = Active
    AlienY = 0
    BulletX = Console.BufferWidth-1
    BulletY = Console.BufferHeight/2
    PlayerX = Console.BufferWidth/4
    PlayerY = Console.BufferHeight/2
    PlayerBulletPosition = Console.BufferWidth/2
    triggerBullet = false
    redrawBullet = false
    redrawScreen = true
    tick = -1 
    clock= 0
}

let updateTick state =
    {state with tick = state.tick+1}
let updateClock state =
    if state.tick % 40 = 0 then 
        {state with clock = state.clock+1;redrawScreen = true}
    else state 
let drawClock state = 
    displayMessageRight 0 ConsoleColor.Yellow $"{state.clock}"
let shoot state = 
    match state.triggerBullet with
    | true -> 
        if state.tick % 1 = 0 then
            {state with PlayerBulletPosition = min (Console.BufferWidth-1)(state.PlayerBulletPosition+1);redrawScreen = true}
        else 
            state
    | _ -> state 
let updateBulletPosition state = 
    if state.redrawBullet then 
        displayMessage state.PlayerBulletPosition state.PlayerY ConsoleColor.Cyan "=>"
        
    

let proccessPlayerKeyboard key state =
    let newState =
        match key with 
        | ConsoleKey.LeftArrow -> {state with PlayerX = max 0 (state.PlayerX-1) }
        | ConsoleKey.RightArrow -> {state with PlayerX = min (Console.BufferWidth-1) (state.PlayerX+1) }
        | ConsoleKey.UpArrow -> {state with PlayerY = max 0 (state.PlayerY-1) }
        | ConsoleKey.DownArrow -> {state with PlayerY = min (Console.BufferWidth-1)(state.PlayerY+1) }
        | ConsoleKey.Escape -> {state with programState = Terminated}
        | ConsoleKey.Spacebar -> {state with redrawBullet = true;triggerBullet = true}
        | _ -> state
    if state <> newState then 
        {newState with redrawScreen = true}
    else state

let drawPlayer (state:State) =
    displayMessage state.PlayerX state.PlayerY ConsoleColor.Black "👽"
let pipeline = [|
    updateTick
    updateClock
    shoot
|]
let redrawPipeline = [|
    drawClock
    drawPlayer
    updateBulletPosition
|]
let myLoop =
    createMainLoop 
        pipeline
        (fun x -> x.programState = Running) 
        redrawPipeline
        [|proccessPlayerKeyboard|]
        (fun x -> x.redrawScreen)
        (fun x -> {x with redrawScreen = false})
// let currentState =
//     myLoop InitialState


let mostrar() =
    let oldForeground = Console.ForegroundColor
    Console.CursorVisible <- false

    let currentState =
        InitialState
        |> myLoop
        
    

    Console.CursorVisible <- true
    Console.ForegroundColor <- oldForeground
    Console.Clear()
    // File.WriteAllText (path,json)
