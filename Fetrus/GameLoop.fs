module GameLoop

let directions = [System.ConsoleKey.LeftArrow; System.ConsoleKey.DownArrow;
                  System.ConsoleKey.RightArrow; System.ConsoleKey.UpArrow]


// TODO find out why this stack overflows
let rec gameLoop state lastTick timeBetweenTicks lastRender timeBetweenRenders =
    
    let nextTick = lastTick + timeBetweenTicks
    let now = System.DateTime.UtcNow
    if now > nextTick then

        printf "Ticked"
        gameLoop (Model.tick state) (lastTick + timeBetweenTicks) timeBetweenTicks lastRender timeBetweenRenders
    let nextRender = lastRender + timeBetweenRenders
    let now = System.DateTime.UtcNow
    if now > nextRender then
        System.Console.SetCursorPosition(0,0)
        System.Console.Write(state.ToString())
        printf "Rendered"
        gameLoop state lastTick timeBetweenTicks (lastRender + timeBetweenRenders) timeBetweenRenders
    if System.Console.KeyAvailable then
        let k = System.Console.ReadKey(true)
        if directions |> List.contains k.Key then
            let action =
                match k.Key with
                | System.ConsoleKey.LeftArrow -> Model.Translate Model.Direction.Left
                | System.ConsoleKey.RightArrow -> Model.Translate Model.Direction.Right
                | System.ConsoleKey.DownArrow -> Model.Translate Model.Direction.Down
                | System.ConsoleKey.UpArrow -> Model.Rotate Model.RotationType.Clockwise
            gameLoop (Model.move state action) lastTick timeBetweenTicks lastRender timeBetweenRenders
    gameLoop state lastTick timeBetweenTicks lastRender timeBetweenRenders