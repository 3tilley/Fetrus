module GameLoopTests

open Model
open ShapeGenerators
open GameLoop

open NUnit.Framework
open FsUnit
    
[<TestFixture>]
type GameLoopTests() = 

    [<Test>]
    member this.``Test game loop`` () =
        
        let gen = ShapeGenerators.SquareFiller()

        let startState = State(Array2D.create 20 10 false, gen.nextBlock 0, gen.nextBlock)

        let now = System.DateTime.UtcNow
        let tickTime = System.TimeSpan.FromMilliseconds(1.0)
        let renderTime = System.TimeSpan.FromSeconds(10.0)

        gameLoop startState now tickTime now renderTime