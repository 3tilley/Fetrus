module Model

open System

type BlockType = | L | Square

type Shape = {
    Coords : (int * int) list
    BlockType : BlockType
}

type State(grid : bool [,], shape : Shape, rng : Random) =
    member x.Grid = grid
    member x.Shape = shape
    member x.Random = rng
    member x.ToString() =
        let s = new System.Text.StringBuilder()
        s.AppendLine() |> ignore
        for i in 1..(x.Grid.GetLength 0) do
            for j in 1..(x.Grid.GetLength 1) do
                let isPiece =
                    x.Shape.Coords
                    |> List.contains (i-1, j-1)
            
                let ascii =
                    if x.Grid.[i-1, j-1] then "#" elif isPiece then "O" else "."

                s.Append(ascii) |> ignore
            s.AppendLine() |> ignore
        s.ToString()

let r = new Random()

let s = new State(Array2D.create 20 10 false, {Coords = [(0,1); (0,2); (1,1); (2,1)]; BlockType = L}, r)

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

let makeNewShape (state : State) =
    let c = (state.Grid.GetLength(1) / 2) - 2
    { Coords = [(0,c); (0, c+1); (1, c); (1, c+1)]; BlockType=Square }

let tick (state : State) : State =

    match (checkDown state) with
    | Some cs ->
        State(state.Grid, { Coords = cs; BlockType = state.Shape.BlockType }, state.Random)
    | None ->
        let newGrid = state.Grid |> Array2D.copy
        
        state.Shape.Coords
        |> List.iter (fun (r,c) -> newGrid.[r,c] <- true )

        let newShape = makeNewShape state

        State(newGrid, newShape, state.Random)

s
|> tick
