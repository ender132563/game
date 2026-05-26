module App.Game
open System
open Generic.Utils
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let options =
    JsonFSharpOptions.Default().ToJsonSerializerOptions()

type ProgramState = 
| Running
| Terminated
| GameOver

type spriteState =
| Alive
| Hit

type misile = {
    x : int
    y : int
}
type State = {
    programState : ProgramState
    playerState : spriteState
    enemyState : spriteState
    enemyX : int
    enemyY : int 
    PlayerMisiles : misile list
    enemyMisiles : misile list
    PlayerX : int 
    PlayerY : int 
    redrawScreen : bool
    tick : int
    clock : int 
    enemyDir : int
    enemyColision : int
    playerColision : int
    canShoot : bool
    score: int
    lives: int
}


let InitialState = {
    programState = Running
    playerState = Alive
    enemyState = Alive
    enemyX = Console.BufferWidth-2
    enemyY = 0
    PlayerMisiles = []
    enemyMisiles = []
    PlayerX = Console.BufferWidth/4
    PlayerY = Console.BufferHeight/2
    redrawScreen = true
    tick = -1 
    clock= -1
    enemyDir = 1
    enemyColision = 0
    playerColision = 0
    canShoot = true
    score = 0
    lives = 3
}

let updateTick state =
    {state with tick = state.tick+1}
let updateClock state =
    if state.tick % 40 = 0 then 
        {state with clock = state.clock+1;redrawScreen = true}
    else state 
let drawClock state = 
    displayMessage (maxX-8) 0 ConsoleColor.Yellow $"Time: {state.clock}"

let drawEnemy state =
    let sprite =
        if state.enemyState = Alive then
            "👾"
        else "💥"
    displayMessage state.enemyX state.enemyY ConsoleColor.Black sprite

let drawPlayer (state:State) =
    let sprite = 
        if state.playerState = Alive then 
            "👽" 
        else "⚰️"
    displayMessage state.PlayerX state.PlayerY ConsoleColor.Black sprite
let drawScore state =
    displayMessage (maxX/2) 0 ConsoleColor.Yellow $"Score: {state.score}"
let drawLives state =
    displayMessage 5 0 ConsoleColor.Yellow $"lives: {state.lives}"
        
let detectPlayerColission state =
    state.enemyMisiles 
    |> Seq.filter (fun misile -> 
        not (misile.x = state.PlayerX-1 && misile.y = state.PlayerY))
    |> Seq.toList
    |> fun newMisiles -> 
        if newMisiles.Length <> state.enemyMisiles.Length then
            {state with 
                playerState = Hit
                enemyMisiles = newMisiles
                redrawScreen=true
                playerColision=state.tick
                }
        else 
            state
let detectEnemyColission state =
    state.PlayerMisiles 
    |> Seq.filter (fun misile -> 
        not (misile.x = state.enemyX-1 && misile.y = state.enemyY))
    |> Seq.toList 
    |> fun newMisiles -> 
        if newMisiles.Length <> state.PlayerMisiles.Length then
            {state with 
                enemyState = Hit
                PlayerMisiles = newMisiles
                redrawScreen=true
                enemyColision=state.tick
                }
        else 
            state

let resetAlien state =
    if state.playerState = Hit then 
        let tiempo = state.tick-state.playerColision
        if tiempo >= 80 then 
            {state with playerState=Alive;redrawScreen=true;lives = state.lives-1}
        else  
            state
    else
        state

let resetEnemigo state =
    if state.enemyState = Hit then 
        let tiempo = state.tick-state.enemyColision
        if tiempo >= 80 then 
            {state with enemyState=Alive;redrawScreen=true;enemyY = random.Next maxY;score = state.score+1}
        
            
  
        else
            
            state
    else
        state

let agregarMisilenemigo state =
    if state.enemyState = Alive && state.tick % 10 = 0 then 
        let newMisile = {
            x = state.enemyX
            y = state.enemyY
        }
        
        {state with  enemyMisiles = newMisile :: state.enemyMisiles}
    else state
let updatePlayerMisile state =
    if state.PlayerMisiles <> [] then 
        state.PlayerMisiles
        |> Seq.map (fun misil -> {misil with x=misil.x+1})
        |> Seq.filter (fun misil -> misil.x < Console.BufferWidth-2)
        |> Seq.toList
        |> fun nuevosMisiles ->
            {state with PlayerMisiles = nuevosMisiles;redrawScreen=true} 
    else state
let updateEnemyMisiles state =
    if state.enemyMisiles <> [] then 
        state.enemyMisiles
        |> Seq.map (fun misil -> {misil with x=misil.x-1})
        |> Seq.filter (fun misil -> misil.x >= 0)
        |> Seq.toList
        |> fun nuevosMisiles ->
            {state with enemyMisiles = nuevosMisiles;redrawScreen=true} 
    else
        state

let drawPlayerBullet state = 
    state.PlayerMisiles
    |> List.iter (fun misile ->
        displayMessage misile.x misile.y  ConsoleColor.Cyan "=>")
let drawEnemyBullet state = 
    state.enemyMisiles
    |> List.iter (fun misile ->
        displayMessage misile.x misile.y  ConsoleColor.Cyan "<=")

        
let proccessGameKeyboard key state =
    match key with
    | ConsoleKey.Escape -> {state with programState = Terminated} 
    | _ -> state
let resetShoot state =
    if not Console.KeyAvailable then
        {state with canShoot = true}
    else
        state

let proccessPlayerKeyboard key state =
    if state.playerState = Alive then 
        match key with 
        | ConsoleKey.Spacebar when state.canShoot ->
            let newMisile = {
                x = state.PlayerX+2
                y = state.PlayerY
            }
            {state  with PlayerMisiles = newMisile :: state.PlayerMisiles; canShoot = false}
        | ConsoleKey.Spacebar -> state
        | ConsoleKey.LeftArrow|ConsoleKey.A -> {state with PlayerX = max 0 (state.PlayerX-2) }
        | ConsoleKey.RightArrow|ConsoleKey.D -> {state with PlayerX = min (Console.BufferWidth-2) (state.PlayerX+2) }
        | ConsoleKey.UpArrow|ConsoleKey.W -> {state with PlayerY = max 0 (state.PlayerY-1) }
        | ConsoleKey.DownArrow|ConsoleKey.S -> {state with PlayerY = min (Console.BufferHeight-1)(state.PlayerY+1) }
        | _ -> state
        |> fun newState ->
            if state <> newState then 
                {newState with redrawScreen = true}
            else state
    else state
let updateEnemyPosition state =
    if state.enemyState= Alive && state.tick % 4 = 0 then 
        let nuevaY = state.enemyY+state.enemyDir
        match nuevaY with 
        | y when y > Console.BufferHeight-1 -> Console.BufferHeight-1,-1
        | y when y < 0 -> 0,1
        | y -> y, state.enemyDir
        |> fun (y,dir) ->
            {state with enemyY=y;enemyDir=dir;redrawScreen=true}
    else
        state

let pipeline = [|
    updateTick
    updateClock
    resetShoot
    updatePlayerMisile
    updateEnemyMisiles
    updateEnemyPosition
    agregarMisilenemigo
    detectPlayerColission
    detectEnemyColission
    resetAlien
    resetEnemigo
    fun state -> 
        if state.lives < 0 then 
            {state with programState = GameOver}
        else state
|]
let redrawPipeline = [|
    drawClock
    drawEnemy
    drawPlayer
    drawEnemyBullet
    drawPlayerBullet
    drawLives
    drawScore
|]
let myLoop =
    createMainLoop 
        pipeline
        (fun x -> if x.programState = Running then true else false) 
        redrawPipeline
        [|proccessPlayerKeyboard;proccessGameKeyboard|]
        (fun x -> x.redrawScreen)
        (fun x -> {x with redrawScreen = false})
    |> fun x -> x



let mostrar state =
    let oldForeground = Console.ForegroundColor
    Console.CursorVisible <- false

    let currentState =
        state 
        |> myLoop
    
    let stateForSaving =
        currentState |> fun s -> {s with programState = Running}

    let json = JsonSerializer.Serialize(stateForSaving, options)
    Console.CursorVisible <- true
    Console.ForegroundColor <- oldForeground
    Console.Clear()
    File.WriteAllText (cache,json)
    currentState.programState 

// let continuar =
//     JsonSerializer.Deserialize<State>(continuePath,options)
    
 
    
