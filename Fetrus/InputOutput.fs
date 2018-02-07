module InputOutput

open Model

let directions = [System.ConsoleKey.LeftArrow; System.ConsoleKey.DownArrow;
                  System.ConsoleKey.RightArrow; System.ConsoleKey.UpArrow]

type ConsoleOut =
    | ConsoleOut
    with static member Render state =
        System.Console.SetCursorPosition(0,0)
        System.Console.Write(state.ToString())

type ConsoleIn =
    | ConsoleIn
    with static member checkKeyInput state =
        match System.Console.KeyAvailable with
        | true ->
            let k = System.Console.ReadKey(true)
            if directions |> List.contains k.Key then
                Some(
                    match k.Key with
                    | System.ConsoleKey.LeftArrow -> Model.Translate Model.Direction.Left
                    | System.ConsoleKey.RightArrow -> Model.Translate Model.Direction.Right
                    | System.ConsoleKey.DownArrow -> Model.Translate Model.Direction.Down
                    | System.ConsoleKey.UpArrow -> Model.Rotate Model.RotationType.Clockwise )
            else
                None
        | false -> None

type DummyOut =
    | DummyOut
    with static member Render state =
        ()

type DummyIn =
    | DummyIn
    with static member checkKeyInput () =
        None

let inline render (output : ^t) (state : State) =
    (^t: (static member Render: State -> unit) state)

let inline checkKeyInput (input : ^t) (state : State) =
    (^t: (static member checkKeyInput: ^t -> State->  Model.Key option) input, state)

let a = ConsoleOut
let b = ConsoleIn
//render a
//checkKeyInput b
//|> ignore