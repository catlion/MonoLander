module LanderMania.Agents

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open System

let ZeroVect = Vector2.Zero

type Textured<'T>(m : ContentManager, res : string, a : 'T) = 
    member __.Texture : Texture2D = m.Load(res)
    member __.Agent = a

type Ship = 
    { Mass : float
      mutable Pos : Vector2
      mutable Speed : Vector2
      mutable Accel : Vector2
      mutable Torque : Vector2 }

type Planet = 
    { Gravity : float32
      Surface : Vector2 array }

module Vector3 = 
    let rotateX angle vector = Vector3.Transform(vector, Matrix.CreateRotationX(angle))
    let rotateY angle vector = Vector3.Transform(vector, Matrix.CreateRotationY(angle))
    let rotateZ angle vector = Vector3.Transform(vector, Matrix.CreateRotationZ(angle))

[<AutoOpen>]
module FunctionalHelpers = 
    let inline radians a = MathHelper.ToRadians(a)
    let inline degrees a = MathHelper.ToDegrees(a)

let generateTetrahedron size = 
    let circumSphereRadius = sqrt (3.f / 8.f) * size
    let centerVertexAngle = acos (-1.f / 3.f)
    let v1 = Vector3(0.f, circumSphereRadius, 0.f)
    let v2 = v1 |> Vector3.rotateX centerVertexAngle
    let v3 = v2 |> Vector3.rotateY (radians 120.f)
    let v4 = v2 |> Vector3.rotateY (-radians 120.f)
    let uv1 = Vector2(0.5f, 1.f - sqrt 0.75f)
    let uv2 = Vector2(0.75f, 1.f - (sqrt 0.75f) / 2.f)
    let uv3 = Vector2(0.25f, 1.f - (sqrt 0.75f) / 2.f)
    let uv4 = Vector2(0.5f, 1.f)
    let uv5 = Vector2.UnitY
    let uv6 = Vector2.One
    [| VertexPositionTexture(v1, uv1)
       VertexPositionTexture(v3, uv2)
       VertexPositionTexture(v2, uv3)
       VertexPositionTexture(v1, uv2)
       VertexPositionTexture(v4, uv6)
       VertexPositionTexture(v3, uv4)
       VertexPositionTexture(v1, uv3)
       VertexPositionTexture(v2, uv4)
       VertexPositionTexture(v4, uv5)
       VertexPositionTexture(v2, uv3)
       VertexPositionTexture(v3, uv2)
       VertexPositionTexture(v4, uv4) |]

type LineDrawer(ai : Vector2 array) = 
    member __.Lsi = Array.init (ai.Length) id
    member val gd : GraphicsDevice = null with get, set
    member val eff : BasicEffect = null with get, set
    member val Lss = Array.zeroCreate<VertexPositionColor> (ai.Length) with get, set
    member val VertDecl : VertexBuffer = null with get, set
    
    member __.Initialize(gd : GraphicsDevice, w : float32, h : float32) = 
        let tc = 
            [| Vector2.Zero
               Vector2(1.f, 1.f)
               Vector2(0.f, 1.f) |]
        do __.gd <- gd
           ai 
           |> Array.iteri 
                  (fun i x -> do __.Lss.[i] <- VertexPositionColor(Vector3(x, 0.f), Color.White))
           __.eff <- new BasicEffect(__.gd)
           __.eff.VertexColorEnabled <- true
           __.eff.EnableDefaultLighting()
           __.eff.World <- Matrix.Identity
           __.eff.View <- Matrix.CreateLookAt(Vector3(0.f, 0.f, 3.f), Vector3.Zero, Vector3.Up)
           __.eff.Projection <- Matrix.CreatePerspectiveFieldOfView
                                    (MathHelper.PiOver4, __.gd.Viewport.AspectRatio, 0.001f, 1000.f)
           __.VertDecl <- new VertexBuffer(__.gd, typeof<VertexPositionColor>, ai.Length, 
                                           BufferUsage.None)
           __.VertDecl.SetData<VertexPositionColor>(__.Lss)
           __.gd.SetVertexBuffer(__.VertDecl)
    
    member __.Draw() = 
        __.eff.EnableDefaultLighting()
        for pass in __.eff.CurrentTechnique.Passes do
            pass.Apply()
            //            __.gd.DrawUserIndexedPrimitives<VertexPositionColor>
            //                (PrimitiveType.TriangleStrip, __.Lss, 0, ai.Length, __.Lsi, 0, __.Lsi.Length)
            __.gd.DrawPrimitives(PrimitiveType.TriangleList, 0, __.Lss.Length)
    
    member __.Dispose() = (__ :> IDisposable).Dispose()
    interface IDisposable with
        member __.Dispose() = __.VertDecl.Dispose()
