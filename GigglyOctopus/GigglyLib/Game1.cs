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
    public enum RoundState
    {
        Player,
        PlayerSimulate,
        AI,
        AISimulate,
        TurnVisualiser,
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world = new World();

        SequentialSystem<float> simulateSys;
        SequentialSystem<float> playerInputSys;
        SequentialSystem<float> visualiserSys;
        SequentialSystem<float> AISys;
        SequentialSystem<float> particleSeqSys;
        SequentialSystem<float> drawSys;

        Entity _player;

        public static RoundState RoundState = RoundState.Player;

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

            _player = world.CreateEntity();
            _player.Set(new CPlayer());
            _player.Set(new CGridPosition());
            _player.Set(new CMovable());
            _player.Set(new CSprite { 
                Texture = Content.Load<Texture2D>("Sprites/player"), 
                Transparency = 0.1f, 
                Depth = 1  
            });

            var bgTexture1 = Content.Load<Texture2D>("Sprites/bg-stars-1");
            var background1 = world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 1.2f });
            background1.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture1,
                Transparency = 0.3f
            });
            background1.Set(new CSourceRectangle { 
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture1.Width, Config.ScreenHeight + bgTexture1.Height) 
            });

            var bgTexture2 = Content.Load<Texture2D>("Sprites/bg-stars-2");
            var background2 = world.CreateEntity();
            background2.Set(new CParallaxBackground { ScrollVelocity = 1.5f });
            background2.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture2,
                Transparency = 0.1f
            });
            background2.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture2.Width, Config.ScreenHeight + bgTexture2.Height)
            });

            var bgTexture3 = Content.Load<Texture2D>("Sprites/bg-stars-3");
            var background3 = world.CreateEntity();
            background3.Set(new CParallaxBackground { ScrollVelocity = 0.6f });
            background3.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture3,
                Transparency = 0.7f
            });
            background3.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture3.Width, Config.ScreenHeight + bgTexture3.Height)
            });

            var bgTexture4 = Content.Load<Texture2D>("Sprites/bg-stars-4");
            var background4 = world.CreateEntity();
            background4.Set(new CParallaxBackground { ScrollVelocity = 0.9f });
            background4.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture4,
                Transparency = 0.5f
            });
            background4.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture4.Width, Config.ScreenHeight + bgTexture4.Height)
            });

            var gridTexture = Content.Load<Texture2D>("Sprites/grid");
            var grid = world.CreateEntity();
            grid.Set(new CParallaxBackground { 
                ScrollVelocity = 6,
                OffsetY = gridTexture.Height / 2 - 0.5f,
                OffsetX = 15.5f
            });
            grid.Set(new CSprite
            {
                X = 15.5f - Config.ScreenWidth / 2,
                Y = gridTexture.Height / 2 - 0.5f - Config.ScreenHeight / 2,
                Texture = gridTexture
            });
            grid.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + gridTexture.Width, Config.ScreenHeight + gridTexture.Height)
            });

            var enemy = world.CreateEntity();
            enemy.Set(new CEnemy());
            enemy.Set(new CGridPosition { X = 10, Y = 10, Facing = Direction.WEST});
            enemy.Set(new CMovable());
            enemy.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/enemy"), Depth = 1, X = 10*Config.TileSize, Y = 10*Config.TileSize, });

            drawSys = new SequentialSystem<float>(
                new RenderingSys(world, spriteBatch)
            );

            particleSeqSys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world)
            );

            playerInputSys = new SequentialSystem<float>(
                new InputSys(world)
            );

            AISys = new SequentialSystem<float>(
                new AISys(world)
            );

            visualiserSys = new SequentialSystem<float>(
                new MoverSys(world),
                new ParallaxSys(world, _player),

                // this should go last
                new EndVisualiseStateSys(world)
            );

            simulateSys = new SequentialSystem<float>(
                new MoveStateSys(world)
                //new AttackStateSys(world)
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

            particleSeqSys.Update(0.0f);
            switch (RoundState)
            {
                case RoundState.Player:
                    playerInputSys.Update(0.0f);
                    if (RoundState == RoundState.AI)
                        goto case RoundState.AI;
                    break;
                case RoundState.PlayerSimulate:
                    simulateSys.Update(0.0f);
                    RoundState = RoundState.AI;
                    goto case RoundState.AI;
                case RoundState.AI:
                    AISys.Update(0.0f);
                    if (RoundState == RoundState.AISimulate)
                        goto case RoundState.AISimulate;
                    break;
                case RoundState.AISimulate:
                    simulateSys.Update(0.0f);
                    RoundState = RoundState.TurnVisualiser;
                    goto case RoundState.TurnVisualiser;
                case RoundState.TurnVisualiser:
                    visualiserSys.Update(0.0f);
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

            (float x, float y) = (-_player.Get<CSprite>().X + 640, -_player.Get<CSprite>().Y + 360);
            var matrix = Matrix.CreateTranslation(x, y, 0);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.LinearWrap, null, null, null, matrix);
            drawSys.Update(0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}