module Generic.Menu

open System
open System.Threading

//
// Esta linea es para traer los simbolos
// del module App.Utils
//
open Generic.Utils

Console.Clear()
type MenuState =
| Active
| Terminated


type State<'C> = {
    MenuState: MenuState
    X: int
    Y: int
    CurSorSelection: int
    CursorX: int
    Commands: ('C * string) array
    color : ConsoleColor
    RedrawScreen: bool
    cursorSymbol : string
    title : string
    titleColor : ConsoleColor
    titlePosition : int * int
}

let initialState x y command Cursor color title titleColor TitlePosition= {
    MenuState = Active
    X = x
    Y = y
    CursorX = x-3
    CurSorSelection = 0
    Commands = command
    color = color
    RedrawScreen = true
    cursorSymbol = Cursor
    title = title
    titleColor = titleColor
    titlePosition = TitlePosition
}

let drawMenu state =
    state.Commands
    |> Array.iteri (fun i (_,legend) ->
        displayMessage state.X (state.Y+i) state.color legend
    )

    displayMessage state.CursorX (state.Y+state.CurSorSelection) ConsoleColor.Yellow state.cursorSymbol
let drawTitle state =

    displayMessage (fst state.titlePosition) (snd state.titlePosition) state.titleColor state.title

let redrawScreen state =
    if state.RedrawScreen then
        Console.Clear()
        drawTitle state
        drawMenu state
        {state with RedrawScreen = false}
    else
        state

let updateMenuKeyboard key state =
    let newState =
        match key with 
        | ConsoleKey.UpArrow -> 
            if state.CurSorSelection = 0 then 
                {state with CurSorSelection = state.Commands.Length-1}
            else    
                {state with CurSorSelection = state.CurSorSelection-1}
        | ConsoleKey.DownArrow -> 
            if state.CurSorSelection = state.Commands.Length-1 then
                {state with CurSorSelection = 0}
            else 
                {state with CurSorSelection = state.CurSorSelection+1}
        | ConsoleKey.Enter -> {state with MenuState = Terminated}
        | _ -> state

    if newState <> state then 
        {newState with RedrawScreen = true}
    else
        state

let processKeyboard state =
    if Console.KeyAvailable then 
        let k = Console.ReadKey true
        state
        |> updateMenuKeyboard k.Key
    else
        state
let rec mainLoop state =
    let newState = 
        state
        |> processKeyboard
        |> redrawScreen
    if newState.MenuState = Active then
        Thread.Sleep 25
        mainLoop newState
    else
        state

let genericMenu x y command cursorSymbol color title titleColor titlePosition =
    let oldForeground = Console.ForegroundColor
    Console.CursorVisible <- false

    let state =
        initialState x y command cursorSymbol color title titleColor titlePosition
        |> mainLoop 

    Console.CursorVisible <- true
    Console.ForegroundColor <- oldForeground
    Console.Clear()
    fst state.Commands[state.CurSorSelection]