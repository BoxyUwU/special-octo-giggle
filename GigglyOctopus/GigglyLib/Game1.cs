using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Systems;
using GigglyLib.Components;
using System;

namespace GigglyLib
{
    public enum TurnState
    {
        Player,
        AI,
        Action,
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world = new World();

        SequentialSystem<float> playerInputSys;
        SequentialSystem<float> actionSys;
        SequentialSystem<float> AISys;

        SequentialSystem<float> drawSys;

        public static TurnState TurnState = TurnState.Player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Config.ScreenWidth;
            graphics.PreferredBackBufferHeight = Config.ScreenHeight;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var player = world.CreateEntity();
            player.Set(new CPlayer());
            player.Set(new CGridPosition());
            player.Set(new CMovable());
            player.Set(new CSprite { 
                Texture = Content.Load<Texture2D>("Sprites/player"), 
                Transparency = 0.1f, 
                Depth = 1  
            });

            var bgTexture1 = Content.Load<Texture2D>("Sprites/bg-stars-1");
            var background1 = world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 0.6f });
            background1.Set(new CSprite { 
                X = bgTexture1.Width/2,
                Y = bgTexture1.Height/2,
                Texture = bgTexture1,
                Transparency = 0.3f
            });
            background1.Set(new CSourceRectangle { 
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture1.Width, Config.ScreenHeight + bgTexture1.Height) 
            });

            var bgTexture2 = Content.Load<Texture2D>("Sprites/bg-stars-2");
            var background2 = world.CreateEntity();
            background2.Set(new CParallaxBackground { ScrollVelocity = 0.8f });
            background2.Set(new CSprite
            {
                X = bgTexture2.Width / 2,
                Y = bgTexture2.Height / 2,
                Texture = bgTexture2,
                Transparency = 0.1f
            });
            background2.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture2.Width, Config.ScreenHeight + bgTexture2.Height)
            });

            var bgTexture3 = Content.Load<Texture2D>("Sprites/bg-stars-3");
            var background3 = world.CreateEntity();
            background3.Set(new CParallaxBackground { ScrollVelocity = 0.2f });
            background3.Set(new CSprite
            {
                X = bgTexture3.Width / 2,
                Y = bgTexture3.Height / 2,
                Texture = bgTexture3,
                Transparency = 0.7f
            });
            background3.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture3.Width, Config.ScreenHeight + bgTexture3.Height)
            });

            var bgTexture4 = Content.Load<Texture2D>("Sprites/bg-stars-4");
            var background4 = world.CreateEntity();
            background4.Set(new CParallaxBackground { ScrollVelocity = 0.4f });
            background4.Set(new CSprite
            {
                X = bgTexture4.Width / 2,
                Y = bgTexture4.Height / 2,
                Texture = bgTexture4,
                Transparency = 0.5f
            });
            background4.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture4.Width, Config.ScreenHeight + bgTexture4.Height)
            });

            var enemy = world.CreateEntity();
            enemy.Set(new CEnemy());
            enemy.Set(new CGridPosition { X = 10, Y = 10, Facing = Direction.WEST});
            enemy.Set(new CMovable());
            enemy.Set(new CMoving());
            enemy.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/enemy"), Depth = 1, X = 10*Config.TileSize, Y = 10*Config.TileSize, });

            drawSys = new SequentialSystem<float>(
                new RenderingSys(world, spriteBatch)
            );

            playerInputSys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                new GridTransformSys(world),
                new InputSys(world),
                new ParallaxSys(world, player)
            );

            actionSys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                new GridTransformSys(world),
                new MoverSys(world),
                new EndActionStateSys(world),
                new ParallaxSys(world, player)
            );

            AISys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                //new GridTransformSys(world),
                new AISys(world),
                new ParallaxSys(world, player)
            );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                
            switch (TurnState)
            {
                case TurnState.Player:
                    playerInputSys.Update(0.0f);
                    break;
                case TurnState.AI:
                    AISys.Update(0.0f);
                    break;
                case TurnState.Action:
                    actionSys.Update(0.0f);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(15, 15, 15));

            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.LinearWrap, null, null);
            drawSys.Update(0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}