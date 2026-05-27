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
| YouWin

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
    clock= 40
    enemyDir = 1
    enemyColision = 0
    playerColision = 0
    canShoot = true
    score = 5
    lives = 3
}

let updateTick state =
    {state with tick = state.tick+1}
let updateClock state =
    if state.tick % 40 = 0 then 
        {state with clock = state.clock-1;redrawScreen = true}
    else state 
let drawClock state = 
    if state.clock >= 0 then
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
    if state.score >= 0 then
        displayMessage (maxX/2) 0 ConsoleColor.Yellow $"EnemyLives: {state.score}"
let drawLives state =
    if state.lives >= 0 then 
        displayMessage 5 0 ConsoleColor.Yellow $"Playerlives: {state.lives}"
        
let applyCollision state missiles hitCondition updateStateWhenHit =
    let newMisiles = missiles |> List.filter (fun misile -> not (hitCondition misile))
    if newMisiles.Length <> missiles.Length then
        updateStateWhenHit newMisiles
    else state

let detectPlayerColission state =
    if state.playerState <> Hit then 
        applyCollision 
            state 
            state.enemyMisiles
            (fun misile -> misile.x = state.PlayerX - 1 && misile.y = state.PlayerY)
            (fun newMisiles ->
                {state with
                    playerState = Hit
                    enemyMisiles = newMisiles
                    redrawScreen = true
                    playerColision = state.tick
                    lives = state.lives-1})
    else state
let detectEnemyColission state =
    if state.enemyState <> Hit then
        applyCollision 
            state 
            state.PlayerMisiles
            (fun misile -> misile.x = state.enemyX - 1 && misile.y = state.enemyY)
            (fun newMisiles ->
                {state with
                    enemyState = Hit
                    PlayerMisiles = newMisiles
                    redrawScreen = true
                    enemyColision = state.tick
                    score = state.score-1})
    else state

let resetAlien state =
    if state.playerState = Hit then 
        let tiempo = state.tick-state.playerColision
        if tiempo >= 80 then 
            {state with playerState=Alive;redrawScreen=true}
        else  
            state
    else
        state

let resetEnemigo state =
    if state.enemyState = Hit then 
        let tiempo = state.tick-state.enemyColision
        if tiempo >= 80 then 
            {state with enemyState=Alive;redrawScreen=true;enemyY = random.Next maxY}
        else
            
            state
    else
        state

let agregarMisilenemigo state =
    if state.enemyState = Alive && state.tick % 10 = 0 then 
        let newMisile = {
            x = state.enemyX-2
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
        | ConsoleKey.RightArrow|ConsoleKey.D -> {state with PlayerX = min (Console.BufferWidth-3) (state.PlayerX+2) }
        | ConsoleKey.UpArrow|ConsoleKey.W -> {state with PlayerY = max 1 (state.PlayerY-1) }
        | ConsoleKey.DownArrow|ConsoleKey.S -> {state with PlayerY = min (Console.BufferHeight-1)(state.PlayerY+1) }
        | _ -> state
        |> fun newState ->
            if state <> newState then 
                {newState with redrawScreen = true}
            else state
    else state
let updateEnemyPosition state =
    if state.enemyState = Alive then 
        let height = float Console.BufferHeight+1.0
        let nuevaY = height/2.0 + height/2.2 * Math.Sin(float state.tick * 0.02 ) |> int
        {state with enemyY = nuevaY; redrawScreen = true}
    else
            state
let resetShoot state =
    if not Console.KeyAvailable then
        {state with canShoot = true}
    else
        state

let pipeline = [|
    fun state -> 
        if state.lives < 0 || state.clock < 0 then 
            {state with programState = GameOver}
        else state
    fun state -> 
        if state.score < 0 then 
            {state with programState = YouWin}
        else state
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
|]
let redrawPipeline = [|
    drawClock
    drawLives
    drawScore
    drawEnemy
    drawPlayer
    drawPlayerBullet
    drawEnemyBullet
|]
let myLoop =
    createMainLoop 
        pipeline
        (fun x -> x.programState = Running)
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


 
    
