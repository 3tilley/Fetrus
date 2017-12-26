namespace Fetrus.Test

module Model =

    open NUnit.Framework
    open FsUnit

    open Model

    let removeWhitespace (s : string) =
        s.Trim()
        |> String.filter (System.Char.IsSeparator >> not)

    let constantBlockGen cols =
        { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = Square; Origin = (0.5, 0.5) }

    let constState s =
        State(s, constantBlockGen)

    [<TestFixture>]
    type Class1() = 

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

            let shape = {Coords = [(0,0); (1,0); (0,1); (1,1)]; BlockType = L; Origin = (0.5, 0.5)}

            let state = State(grid, shape, constantBlockGen)

            let ticked = tick state

            (ticked.ToString())
            |> should equal ("""
                            ....
                            OO..
                            OO..
                            ....
                            ....
                            ....
                            """ |> removeWhitespace)

        [<Test>]
        member this.``Test read string state`` () =
        
            let testStringState=
                "....
                .OO.
                OO..
                ....
                ....
                ...."
                |> removeWhitespace
        
            State(testStringState, constantBlockGen).ToString()
            |> should equal testStringState

        [<Test>]
        member this.``Test L shape`` () =
            "....
            .OO.
            OO..
            ....
            ....
            ...."
            |> constState
            |> tick
            |> fun s -> s.ToString()
            |> should equal ("....
                            ....
                            .OO.
                            OO..
                            ....
                            ...." |> removeWhitespace)

        [<Test>]
        member this.``Test fixed blocks`` () =
            "....
            .OO.
            OO..
            ....
            ..##
            ####"
            |> constState
            |> tick
            |> fun s -> s.ToString()
            |> should equal ("....
                            ....
                            .OO.
                            OO..
                            ..##
                            ####" |> removeWhitespace)

        [<Test>]
        member this.``Test blocks collide`` () =
            "....
            ....
            ....
            .OO.
            OO#.
            #.##"
            |> constState
            |> tick
            |> fun s -> s.ToString()
            |> should equal ("OO..
                              OO..
                              ....
                              .##.
                              ###.
                              #.##" |> removeWhitespace)

        [<Test>]
        member this.``Test rows destroy themselves`` () =
            "....
            ....
            ....
            .OO.
            OO##
            #.##"
            |> constState
            |> tick
            |> fun s -> s.ToString()
            |> should equal ("OO..
                              OO..
                              ....
                              ....
                              .##.
                              #.##" |> removeWhitespace)

        [<Test>]
        member this.``Test basic move`` () =
            "....
            OO..
            OO..
            ....
            ..##
            #.##"
            |> constState
            |> fun s -> move s (Translate Right)
            |> fun s -> s.ToString()
            |> should equal ("....
                              .OO.
                              .OO.
                              ....
                              ..##
                              #.##" |> removeWhitespace)

        [<Test>]
        member this.``Test move into wall`` () =
            "....
            OO..
            OO..
            ....
            ..##
            #.##"
            |> constState
            |> fun s -> move s (Translate Left)
            |> fun s -> s.ToString()
            |> should equal ("....
                              OO..
                              OO..
                              ....
                              ..##
                              #.##" |> removeWhitespace)