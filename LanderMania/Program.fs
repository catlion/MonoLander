module LanderMania.Program

open Agents
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
      Surface = 
          [| Vector2(-1.f, 1.f)
             Vector2(1.f, -1.f)
             Vector2(-1.f, -1.f) |] }

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
        rs.CullMode <- CullMode.None
        rs.FillMode <- FillMode.WireFrame
        //this.GraphicsDevice.RasterizerState <- rs
        base.Initialize()
    
    /// <summary> </summary>
    override this.LoadContent() = 
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        ship <- Textured<Ship>(this.Content, "ship.png", defaultShip)
        planet <- Textured<Planet>(this.Content, "meteor.png", defaultPlanet)
        sfcv.Initialize(this.GraphicsDevice, w, h)
        base.LoadContent()
    
    /// <summary> </summary>
    override this.Update(gameTime) = Physics.addGravity planet.Agent ship.Agent
    
    /// <summary> </summary>
    override this.Draw(gameTime) = 
        this.GraphicsDevice.Clear Color.CadetBlue
        sfcv.Draw()
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend)
        spriteBatch.Draw(planet.Texture, Rectangle(0, h - 100.0f |> int, int w, 100), Color.White)
        spriteBatch.Draw(ship.Texture, ship.Agent.Pos, Color.White)
        spriteBatch.End()
        base.Draw(gameTime)
    
    interface IDisposable with
        member __.Dispose() = 
            do rs.Dispose()
               sfcv.Dispose()

/// <summary> </summary>
do use game = new Lander()
   game.Run()
