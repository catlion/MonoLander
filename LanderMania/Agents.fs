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

type LineDrawer(ai : Vector2 array) = 
    let mutable gd : GraphicsDevice = null
    let mutable eff : BasicEffect = null
    let mutable verts = Array.zeroCreate<VertexPositionColor> (ai.Length)
    
    //let mutable vbuf : VertexBuffer = null
    member __.Initialize(gd' : GraphicsDevice, w : float32, h : float32) = 
        do gd <- gd'
           ai 
           |> Array.iteri 
                  (fun i x -> do verts.[i] <- VertexPositionColor(Vector3(x, 0.f), Color.White))
           eff <- new BasicEffect(gd)
           eff.VertexColorEnabled <- true
           eff.World <- Matrix.Identity
           eff.View <- Matrix.CreateLookAt(Vector3(0.f, 0.f, 10.f), Vector3.Zero, Vector3.Up)
           eff.Projection <- Matrix.CreatePerspectiveFieldOfView
                                 (MathHelper.PiOver4, gd.Viewport.AspectRatio, 1.f, 1000.f)
    
    //           vbuf <- new VertexBuffer(gd, VertexPositionColor.VertexDeclaration, ai.Length, 
    //                                    BufferUsage.WriteOnly)
    //           vbuf.SetData(verts)
    //           gd.SetVertexBuffer(vbuf)
    member __.Draw() = 
        for pass in eff.CurrentTechnique.Passes do
            pass.Apply()
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, verts.Length / 3)
    
    member __.Dispose() = (__ :> IDisposable).Dispose()
    interface IDisposable with
        member __.Dispose() = 
            //vbuf.Dispose()
            ()
