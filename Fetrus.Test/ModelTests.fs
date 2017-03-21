namespace Fetrus.Test

open NUnit.Framework
open FsUnit

open Model

[<TestFixture>]
type Class1() = 
    [<Test>]
    member this.``First test`` () =
        "F#" |> should equal "F#"

    [<Test>]
    member this.``Test basic tick`` () =
        let grid =
            [[false; false; false; false]
             [false; false; false; false]
             [false; false; false; false]
             [false; false; false; false]
             [false; false; false; false]
             [false; false; false; false]]
             |> array2D

        let shape = {Coords = [(0,0); (1,0); (0,1); (1,1)]; BlockType = L}

        let state = State(grid, shape, null)

        let ticked = tick state

        (ticked.ToString())
        |> should equal """
....
OO..
OO..
....
....
....
"""

    [<Test>]
    member this.``Test read string state`` () =
        
        let testStringState=
            """
....
.OO.
OO..
....
....
....
"""
        
        State(testStringState).ToString()
        |> should equal testStringState

    [<Test>]
    member this.``Test L shape`` () =
        """
....
.OO.
OO..
....
....
....
"""     |> State
        |> tick
        |> fun s -> s.ToString()
        |> should equal """
....
....
.OO.
OO..
....
....
"""