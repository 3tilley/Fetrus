﻿module Model

open System

type BlockType = | L | Square

type Shape = {
    Coords : (int * int) list
    BlockType : BlockType
}

type State(grid : bool [,], shape : Shape, newBlockGenerator) =
    member x.Grid = grid
    member x.Shape = shape
    member x.Rows = grid.GetLength 0
    member x.Cols = grid.GetLength 1
    member x.BlockGenerator = newBlockGenerator
    member x.ToString() =
        let s = new System.Text.StringBuilder()
        //s.AppendLine() |> ignore
        for i in 1..(x.Grid.GetLength 0) do
            for j in 1..(x.Grid.GetLength 1) do
                let isPiece =
                    x.Shape.Coords
                    |> List.contains (i-1, j-1)
            
                let ascii =
                    if x.Grid.[i-1, j-1] then "#" elif isPiece then "O" else "."

                s.Append(ascii) |> ignore
            s.AppendLine() |> ignore
        s.ToString().Trim()
    new(s : string, blockGen) =
        let strings =
            s.Split([|"\n"; "\r\n"; " "|], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun i -> i.Trim())

        let rows = strings.Length
        let cols = strings.[0].Length

        let lst = System.Collections.Generic.List()

        let grid =
            Array2D.init rows cols (fun r c ->
                if strings.[r].Length <> cols then
                    failwith "Input string jagged"
                match strings.[r].[c] with
                | '.' -> false
                | '#' -> true
                | 'O' ->
                    lst.Add((r,c))
                    false)
        // TODO: Everything is initiated as a block. Horrid!!!!
        let shape = { Coords = lst |> List.ofSeq; BlockType = L }
        State(grid, shape, blockGen)

let checkDown (state : State) =
    let newCoords =
        state.Shape.Coords
        |> List.map (fun (r, c) -> (r + 1, c))

    let isClear =
        newCoords
        |> List.map (fun (r, c) -> state.Grid.[r, c])
        |> List.forall not

    match isClear with
    | false -> None
    | true -> Some(newCoords)

let tick (state : State) : State =

    match (checkDown state) with
    | Some cs ->
        State(state.Grid, { Coords = cs; BlockType = state.Shape.BlockType }, state.BlockGenerator)
    | None ->
        let newGrid = state.Grid |> Array2D.copy
        
        state.Shape.Coords
        |> List.iter (fun (r,c) -> newGrid.[r,c] <- true )

        let newShape = state.BlockGenerator state.Cols

        // This is to remove complete rows
        let newRows =
            [ for r in 0..(state.Rows-1) ->
                [for c in 0..(state.Cols-1) -> newGrid.[r,c]] ]
            |> List.filter (fun lst -> lst |> List.forall id |> not)

        let filteredGrid =
            match newRows.Length with
            | x when x = state.Rows -> newGrid
            | y when y < state.Rows ->
                let emptyRows =
                    List.replicate (state.Rows - y) (List.replicate state.Cols false)
                newRows
                |> List.append emptyRows
                |> array2D
            | x ->
                failwithf "Number of rows increased after filter %i to %i" state.Rows x
        State(filteredGrid, newShape, state.BlockGenerator)
