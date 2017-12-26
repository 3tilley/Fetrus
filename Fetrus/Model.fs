module Model

open System

type BlockType = | L | Square

type Shape = {
    Coords : (int * int) list
    BlockType : BlockType
    Origin : double * double
}

type Direction = | Left | Right | Down
type RotationType = | Clockwise | AntiClockwise

type Key =
    | Translate of Direction
    | Rotate of RotationType

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
        let cs = lst |> List.ofSeq
        let o = (cs |> List.averageBy (fst >> double), cs |> List.averageBy (snd >> double))
        let shape = { Coords = lst |> List.ofSeq; BlockType = L; Origin = o }
        State(grid, shape, blockGen)

let isValidState newCoords (state : State) =

        newCoords
        |> List.map (fun (r, c) ->
            if ((r >= 0) && (c >= 0) && (r < state.Rows) && (c < state.Cols)) then
                not state.Grid.[r, c]
            else
                false)
        |> List.forall id


let checkDirection direction (state : State) =

    let coordChecker =
        match direction with
        | Down -> fun (r, c) -> (r + 1, c)
        | Left -> fun (r, c) -> (r, c - 1)
        | Right -> fun (r, c) -> (r, c + 1)

    let originMover =
        match direction with
        | Down -> fun (r, c) -> (r + 1.0, c)
        | Left -> fun (r, c) -> (r, c - 1.0)
        | Right -> fun (r, c) -> (r, c + 1.0)
    
    let newCoords =
        state.Shape.Coords
        |> List.map coordChecker

    match isValidState newCoords state with
    | false -> None
    | true -> Some(newCoords, originMover state.Shape.Origin)

let makeRotatedCoordinates (shape : Shape) rotationType =
    // Using the rotation matrices:
    // [.0  1][x] = [ y]
    // [-1  0][y]   [-x]
    // and
    // [ 0 -1]
    // [ 1..0]
    let originR, originC = shape.Origin 
    let centredCoords =
        shape.Coords
        |> List.map (fun (r, c) -> float(r) - originR, float(c) - originC)

    let rotate = 
        match rotationType with
        | Clockwise -> fun (r, c) -> (c, -r)

    centredCoords
    |> List.map rotate
    |> List.map (fun (r, c) -> int(r + originR), int(c + originC))
        
let tryRotation (state : State) rd =
    let newCoords = makeRotatedCoordinates state.Shape rd
    match isValidState newCoords state with
    | false -> None
    | true -> Some(newCoords)

let tick (state : State) : State =

    match (checkDirection Down state) with
    | Some (cs, o)->
        let tickedShape = {
            Coords = cs
            BlockType = state.Shape.BlockType
            Origin = o
        }
        State(state.Grid, tickedShape, state.BlockGenerator)
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

let move (state : State) (key : Key) =
    
    match key with
    | Translate(d) ->
        match (checkDirection d state) with
        | Some(cs, o) ->
            let movedShape = {
                Coords = cs
                BlockType = state.Shape.BlockType
                Origin = o
            }
            State(state.Grid, movedShape, state.BlockGenerator)
        | None ->
            state
    | Rotate(rd) ->
        match (tryRotation state rd) with
        | None -> state
        | Some(cs) ->
            let rotatedShape = {
                Coords = cs
                BlockType = state.Shape.BlockType
                Origin = state.Shape.Origin
            }
            State(state.Grid, rotatedShape, state.BlockGenerator)

