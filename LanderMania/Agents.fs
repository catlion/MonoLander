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
    let mutable verts = Array.zeroCreate<VertexPositionTexture> (ai.Length)
    
    //let mutable vbuf : VertexBuffer = null
    member __.Initialize(gd' : GraphicsDevice, w : float32, h : float32, t : Texture2D) = 
        let rnd (x: float32) = Math.Round(float x) |> float32
        do gd <- gd'
           ai 
           |> Array.iteri 
                  (fun i x -> 
                  let ii = float32 i
                  do verts.[i] <- VertexPositionTexture
                                      (Vector3(Vector2(rnd(w - x.X), rnd(h - x.Y)), 0.f), 
                                       Vector2.Zero))
                                       //Vector2(1.f / ii, 0.99f / ii)))
           eff <- new BasicEffect(gd)
           //eff.VertexColorEnabled <- true
           eff.TextureEnabled <- true
           eff.Texture <- t
           eff.World <- Matrix.Identity
           eff.View <- Matrix.CreateLookAt(Vector3(0.f, 0.f, 10.f), Vector3.Zero, Vector3.Up)
           let halfPixOff = Matrix.CreateTranslation(0.5f, -0.5f, 0.f)
           eff.Projection <- Matrix.op_Multiply 
                                 (halfPixOff, 
                                  Matrix.CreateOrthographicOffCenter(0.f, w, h, 0.f, 0.1f, 1000.f))
    
    member __.Draw() = 
        for pass in eff.CurrentTechnique.Passes do
            pass.Apply()
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, verts.Length / 3)
    
    member __.Dispose() = (__ :> IDisposable).Dispose()
    interface IDisposable with
        member __.Dispose() = ()
//vbuf.Dispose()
