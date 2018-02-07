module GameLoop

open InputOutput
open Model

// TODO find out why this stack overflows
let rec inline gameLoop state lastTick timeBetweenTicks lastRender
                    timeBetweenRenders
                    input
                    (output : ^t when ^t: (static member Render: State -> unit)) =
    
    let nextTick = lastTick + timeBetweenTicks
    let now = System.DateTime.UtcNow
    if now > nextTick then

        printf "Ticked"
        gameLoop (Model.tick state) (lastTick + timeBetweenTicks) timeBetweenTicks lastRender timeBetweenRenders input output 
    let nextRender = lastRender + timeBetweenRenders
    let now = System.DateTime.UtcNow
    if now > nextRender then
        //let c = InputOutput.ConsoleOut
        InputOutput.render output state
        gameLoop state lastTick timeBetweenTicks (lastRender + timeBetweenRenders) timeBetweenRenders input output 
    if System.Console.KeyAvailable then
        let k = System.Console.ReadKey(true)
        if directions |> List.contains k.Key then
            let action =
                match k.Key with
                | System.ConsoleKey.LeftArrow -> Model.Translate Model.Direction.Left
                | System.ConsoleKey.RightArrow -> Model.Translate Model.Direction.Right
                | System.ConsoleKey.DownArrow -> Model.Translate Model.Direction.Down
                | System.ConsoleKey.UpArrow -> Model.Rotate Model.RotationType.Clockwise
            gameLoop (Model.move state action) lastTick timeBetweenTicks lastRender timeBetweenRenders input output 
    gameLoop state lastTick timeBetweenTicks lastRender timeBetweenRenders input output

let gen = ShapeGenerators.SquareFiller()

let startState = Model.State(Array2D.create 20 10 false, gen.nextBlock 0, gen.nextBlock)

let now = System.DateTime.UtcNow
let tickTime = System.TimeSpan.FromMilliseconds(1.0)
let renderTime = System.TimeSpan.FromSeconds(10.0)

let i = InputOutput.ConsoleIn
let o = InputOutput.ConsoleOut
let o2 = InputOutput.DummyOut
//gameLoop startState now tickTime now renderTime i o
//gameLoop startState now tickTime now renderTime i o2