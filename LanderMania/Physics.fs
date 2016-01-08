module LanderMania.Physics

open LanderMania.Agents
open Microsoft.Xna.Framework
open System.Diagnostics

let scale (v : float32) = v / 300.0f

let addGravity (gameTime: GameTime) (planet : Planet) (ship : Ship) = 
    let g = planet.Gravity |> scale
    ship.Pos <- Vector2.Add(ship.Speed, ship.Pos)
    if ship.Accel.Y < g then ship.Accel.Y <- g
    ship.Speed <- Vector2.Add(ship.Speed, ship.Accel)

let checkCollision (planet : Planet) (ship : Ship) = ()
//planet.Surface.
