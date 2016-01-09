module LanderMania.Landscape

open Microsoft.Xna.Framework
open System

let generateLandscape (x : float32, y : float32) = 
    let stepNum = 10.f
    let inline f (i : int) = float32 i
    let stepSize = x / stepNum
    let maxHeight = y / 5.f |> int
    let rnd = new Random()
    let zero = 0.f
    let halfStep = stepSize / 2.f
    
    let arr = 
        seq { 
            for i in 0..int stepNum do
                let rndDo = rnd.Next(2)
                let height = rnd.Next(maxHeight) + (maxHeight / 2) |> f
                let nearX = stepSize * f i
                let midX = nearX + halfStep + f (rnd.Next(int -halfStep, int halfStep)) / 2.f
                let farX = nearX + stepSize
                yield! [| Vector2(nearX, zero)
                          Vector2(farX, zero)
                          Vector2(midX, height) |]
                if rndDo > 0 then 
                    yield! [| Vector2(farX, zero)
                              Vector2(farX + halfStep * 1.1f, height)
                              Vector2(nearX - halfStep * 1.1f, height) |]
        }
    arr |> Array.ofSeq
