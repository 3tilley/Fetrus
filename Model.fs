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

let r = new Random()

let s = new State(Array2D.create 2 2 false, {Coords = [(1,1)]; BlockType = L}, r)

let checkDown (state : State) =
    let newCoords =
        state.Shape.Coords
        |> List.map (fun (r, c) -> (r + 1, c))

    let isClear =
        newCoords
        |> List.map (fun (r, c) -> state.Grid.[r, c])
        |> List.forall id

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

let print