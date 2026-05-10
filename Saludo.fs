module App.Saludo
open System
open App.Utils
open System.Threading

type ProgramState =
| Running
| Terminated

type keyboardHelper =
| Active
| WithData

type keyboardState = {
    keyboardHelper : keyboardHelper
    data : string
}

let initialKeyboard = {
    keyboardHelper = Active
    data = ""
}

type state = {
    programState : ProgramState
    tick : int
    clock : int
    keyboard : keyboardState
}

let initialState = {
    programState = Running
    tick = 0
    clock = 0
    keyboard = initialKeyboard
}


let displayMessageRight y color (msg:string) =
    let x = max 0 (Console.BufferWidth - msg.Length)
    displayMessage x y color msg

let isAlphanumericKey (keyInfo:ConsoleKeyInfo) =
    let c = keyInfo.KeyChar
    Char.IsLetterOrDigit(c)

let keyBoardInput state (keyInfo:ConsoleKeyInfo) =
    match keyInfo.Key with
    | ConsoleKey.Escape -> { state with programState = Terminated }
    | ConsoleKey.Enter when state.keyboard.data <> "" ->
        { state with keyboard = { state.keyboard with keyboardHelper = WithData } }
    | ConsoleKey.Backspace ->
        let newData =
            if state.keyboard.data.Length > 0 then
                state.keyboard.data.Substring(0, state.keyboard.data.Length - 1)
            else
                state.keyboard.data
        { state with keyboard = { state.keyboard with data = newData; keyboardHelper = Active } }
    | _ when isAlphanumericKey keyInfo ->
        let charToAdd = keyInfo.KeyChar
        { state with keyboard = { state.keyboard with data = state.keyboard.data + string charToAdd; keyboardHelper = Active } }
    | _ -> state

let processKeyboard state =
    if Console.KeyAvailable then
        let keyInfo = Console.ReadKey true
        keyBoardInput state keyInfo
    else
        state

let redrawScreen state =
    displayMessageRight 1 ConsoleColor.DarkRed $"{state.clock}"
    displayMessage 2 3 ConsoleColor.Cyan "Nombre :"
    displayMessage 2 4 ConsoleColor.White state.keyboard.data

    match state.keyboard.keyboardHelper with
    | WithData when state.keyboard.data <> "" ->
        displayMessage 2 6 ConsoleColor.Green $"Hola, {state.keyboard.data}!"
    | _ -> ()
    state

let updateTick state =
    { state with tick = state.tick + 1 }

let updateClock state =
    if state.tick <> 0 && state.tick % 40 = 0 then
        { state with clock = state.clock + 1 }
    else
        state

let rec mainLoop state =
    let state =
        state
        |> updateTick
        |> updateClock
        |> processKeyboard

    redrawScreen state |> ignore

    if state.programState = Running then
        Thread.Sleep 25
        mainLoop state

let mostrar() =
    let old = Console.ForegroundColor
    Console.CursorVisible <- false
    Console.Clear()

    mainLoop initialState

    Console.CursorVisible <- true
    Console.ForegroundColor <- old
    Console.Clear()