// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open Model
open Tetronimoes

[<EntryPoint>]
let main argv =
    System.Console.CursorVisible <- false
    let constantBlockGen cols =
        { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = O; Origin = (0.5, 0.5)}

    let gen = ShapeGenerators.SevenBag()

    let start = Model.State(Array2D.create 20 10 false, constantBlockGen 10, gen.nextBlock)
    let now = System.DateTime.UtcNow
    let renderTime = System.TimeSpan.FromSeconds(1.0 / 10.0)
    let tickTime = System.TimeSpan.FromSeconds(1.0)
    GameLoop.gameLoop start now tickTime now renderTime
    0 // return an integer exit code
