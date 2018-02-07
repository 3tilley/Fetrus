module ShapeGenerators
open Model
open Tetronimoes

let constantBlockGen cols =
    { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = O; Origin = (0.5, 0.5)}

type SquareFiller() =
    let mutable lastColDrop = 0
    let mutable isFirstDrop = true
    member x.nextBlock cols =
        if cols % 2 <> 0 then
            failwith "Must have an even number of columns"
        let nextCol =
            if isFirstDrop then
                isFirstDrop <- false
            else
                if ((lastColDrop + 3) <= cols) then
                    lastColDrop <- lastColDrop + 2
                else
                    lastColDrop <- 0
            lastColDrop
        { Coords = [(0, nextCol); (1, nextCol); (0, nextCol+1); (1, nextCol+1)]; BlockType=O; Origin = (0.5, float(nextCol)+0.5)}

type Alternator() =
    let mutable wasLastBlock = false
    member x.nextBlock cols =
        if wasLastBlock then
            wasLastBlock <- not wasLastBlock
            { Coords = [(1, 0); (1, 1); (1, 2); (0, 2)]; BlockType = L; Origin = (1.0, 1.0) }
        else
            wasLastBlock <- not wasLastBlock
            { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = O; Origin = (0.5, 0.5) }

type SevenBag() =
    let rand = System.Random()
    let remainingBlocks = System.Collections.Generic.Queue<BlockType>()
    member x.nextBlock cols =
        if remainingBlocks.Count = 0 then
            let randList = [for _ in 1..blockList.Length -> rand.Next()]
            blockList
            |> List.mapi (fun i v -> (randList.[i], v))
            |> List.sortBy fst
            |> List.iter (fun (_, v) -> remainingBlocks.Enqueue(v))
        spawn cols (remainingBlocks.Dequeue())