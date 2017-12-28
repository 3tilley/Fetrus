module Tetronimoes

type BlockType = | L | J | S | Z | T | O | I

let blockList = [L; J; S; Z; T; O; I]

type Shape = {
    Coords : (int * int) list
    BlockType : BlockType
    Origin : double * double
}

let spawn cols blockType =
    let mid = (cols / 2) - 1
    match blockType with
    | O ->
        { Coords = [(0, mid); (1, mid); (0, mid+1); (1, mid+1)]
          BlockType = blockType
          Origin = (0.5, (float mid) + 0.5)}
    | I ->
        { Coords = [(1, mid-1); (1, mid); (1, mid+1); (1, mid+2)]
          BlockType = blockType
          Origin = (0.0, (float mid) + 0.5)}
    | T ->
        { Coords = [(1, mid-1); (1, mid); (1, mid+1); (0, mid)]
          BlockType = blockType
          Origin = (1.0, float (mid))}
    | L->
        { Coords = [(1, mid-1); (1, mid); (1, mid+1); (0, mid+1)]
          BlockType = blockType
          Origin = (1.0, (float mid))}
    | J ->
        { Coords = [(0, mid-1); (1, mid-1); (1, mid); (1, mid+1)]
          BlockType = blockType
          Origin = (1.0, (float mid))}
    | S ->
        { Coords = [(1, mid-1); (1, mid); (0, mid); (0, mid+1)]
          BlockType = blockType
          Origin = (1.0, (float mid))}
    | Z ->
        { Coords = [(0, mid-1); (0, mid); (1, mid); (1, mid+1)]
          BlockType = blockType
          Origin = (1.0, (float mid))}