module ShapeGenerators
open Model

    let constantBlockGen cols =
        { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = Square; Origin = (0.5, 0.5)}

    type Alternator() =
        let mutable wasLastBlock = false
        member x.nextBlock cols =
            if wasLastBlock then
                wasLastBlock <- not wasLastBlock
                { Coords = [(1, 0); (1, 1); (1, 2); (0, 2)]; BlockType = L; Origin = (1.0, 1.0) }
            else
                wasLastBlock <- not wasLastBlock
                { Coords = [(0, 0); (0, 1); (1, 0); (1, 1)]; BlockType = Square; Origin = (0.5, 0.5) }