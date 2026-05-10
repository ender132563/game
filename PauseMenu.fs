module App.Pause

open Generic.Menu 

type commands =
| Continue
| SaveGame
| Exit

let menu() =
    genericMenu
        20
        10
        [|
            Continue,"Continue"
            SaveGame,"Save Game"
            Exit,"Exit"
        |]
        "👉"

