[<AutoOpen>]
module LanderMania.Vector2

open Microsoft.Xna.Framework

/// <summary> Kilogram </summary>
[<Measure>]
type kg

/// <summary> Meter </summary>
[<Measure>]
type m

/// <summary> Second </summary>
[<Measure>]
type sec

/// <summary> Unit of measure to float32 </summary>
let inline unit2f (s : float32<_>) = s / 1.f<_>

/// <summary> Create pair of measured x,y from vector, multiplying vector dimensions </summary>
let inline (|*|) (s : Vector2) (uv : float32<_>) = s.X * uv, s.Y * uv

/// <summary> Create pair of measured x,y from vector, adding vector dimensions </summary>
let inline (|+|) (s : Vector2) (uv : float32<_>) = s.X * 1.f<_> + uv, s.Y * 1.f<_> + uv

/// <summary> Vector2 from measured pair of values </summary>
let inline f2vect (x : float32<_>, y : float32<_>) = Vector2(x / 1.f<_>, y / 1.f<_>)
