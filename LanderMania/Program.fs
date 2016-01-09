module LanderMania.Program

open Agents
open Landscape
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open System

let w = 600.f
let h = 600.f

let defaultShip = 
    { Mass = 1.
      Pos = Vector2(100.199f)
      Speed = ZeroVect
      Accel = ZeroVect
      Torque = ZeroVect }

let defaultPlanet = 
    { Gravity = 9.8f
      Surface = generateLandscape (w, h) }
//      Surface = 
//          [| Vector2(0.f, 0.f)
//             Vector2(200.f, 0.f)
//             Vector2(100.f, 100.f) |] }

type Lander() as this = 
    inherit Game()
    let graphics = new GraphicsDeviceManager(this)
    let rs = new RasterizerState()
    let sfcv = new LineDrawer(defaultPlanet.Surface)
    
    do 
        graphics.PreferredBackBufferWidth <- int w
        graphics.PreferredBackBufferHeight <- int h
        graphics.PreferredDepthStencilFormat <- DepthFormat.None
    
    let mutable spriteBatch : SpriteBatch = null
    let mutable ship = Unchecked.defaultof<Textured<Ship>>
    let mutable planet = Unchecked.defaultof<Textured<Planet>>
    
    /// <summary> </summary>
    override this.Initialize() = 
//        rs.CullMode <- CullMode.None
//        rs.FillMode <- FillMode.Solid
//        this.GraphicsDevice.RasterizerState <- rs
        this.GraphicsDevice.VertexSamplerStates.[0] <- SamplerState.PointWrap
        this.GraphicsDevice.SamplerStates.[0] <- SamplerState.PointWrap
        base.Initialize()
    
    /// <summary> </summary>
    override this.LoadContent() = 
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        ship <- Textured<Ship>(this.Content, "ship.png", defaultShip)
        planet <- Textured<Planet>(this.Content, "meteor.png", defaultPlanet)
        sfcv.Initialize(this.GraphicsDevice, w, h)
        base.LoadContent()
    
    /// <summary> </summary>
    override this.Update(gameTime) = Physics.addGravity gameTime planet.Agent ship.Agent
    
    /// <summary> </summary>
    override this.Draw(gameTime) = 
        this.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend)
        //spriteBatch.Draw(planet.Texture, Rectangle(0, h - 100.0f |> int, int w, 100), Color.White)
        spriteBatch.Draw(ship.Texture, ship.Agent.Pos, Color.White)
        spriteBatch.End()
        sfcv.Draw()
        base.Draw(gameTime)
    
    interface IDisposable with
        member __.Dispose() = 
            do rs.Dispose()
               sfcv.Dispose()

/// <summary> </summary>
do use game = new Lander()
   game.Run()
