using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Systems;
using GigglyLib.Components;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using GigglyLib.ProcGen;

namespace GigglyLib
{
    public enum GameState 
    {
        Starting,
        Playing,
    }

    public enum RoundState
    {
        Player,
        AI,
        Simulate,
        PostTurn,
        TurnVisualiser,
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        int _seed;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static World world = new World();

        SequentialSystem<float> simulateSys;
        SequentialSystem<float> playerInputSys;
        SequentialSystem<float> visualiserSys;
        SequentialSystem<float> AISys;
        SequentialSystem<float> particleSeqSys;
        SequentialSystem<float> drawSys;
        SequentialSystem<float> roundPrepSys;

        public static Entity _player;
        public static GameState GameState = GameState.Starting;
        public static int currentRoundState = 0;
        public static RoundState[] roundOrder = new RoundState[]
        {
            RoundState.AI,
            RoundState.Player,
            RoundState.Simulate,
            RoundState.PostTurn,
            RoundState.TurnVisualiser,
        };

        public enum Colour
        {
            RED,
            ORANGE,
            GOLDEN,
            YELLOW,
            LIME,
            LIGHTGREEN,
            GREEN,
            SEAGREEN,
            AQUAMARINE,
            CYAN,
            LIGHTBLUE,
            BLUE,
            VIOLET,
            PURPLE,
            LIGHTPURPLE,
            PINK,
            MAGENTA,
            FUSCHIA
        }

        public static List<Texture2D> PARTICLES = new List<Texture2D>();

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

            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-red"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-orange"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-golden"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-yellow"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-lime"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-lightgreen"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-green"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-seagreen"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-aquamarine"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-cyan"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-lightblue"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-blue"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-violet"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-purple"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-lightpurple"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-pink"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-magenta"));
            PARTICLES.Add(Content.Load<Texture2D>("Sprites/particles-fuschia"));

            Config.Textures.Add("enemy", Content.Load<Texture2D>("Sprites/enemy"));
            Config.Textures.Add("asteroid", Content.Load<Texture2D>("Sprites/asteroid-tile"));
            Config.Textures.Add("player", Content.Load<Texture2D>("Sprites/player"));
            Config.Textures.Add("grid", Content.Load<Texture2D>("Sprites/grid"));
            Config.Textures.Add("bg-stars-1", Content.Load<Texture2D>("Sprites/bg-stars-1"));
            Config.Textures.Add("bg-stars-2", Content.Load<Texture2D>("Sprites/bg-stars-2"));
            Config.Textures.Add("bg-stars-3", Content.Load<Texture2D>("Sprites/bg-stars-3"));
            Config.Textures.Add("bg-stars-4", Content.Load<Texture2D>("Sprites/bg-stars-4"));
            Config.Textures.Add("particles-smoke", Content.Load<Texture2D>("Sprites/particles-smoke"));
            Config.Textures.Add("power-up", Content.Load<Texture2D>("Sprites/power-up"));
        }

        private void CreateSystems()
        {
            drawSys = new SequentialSystem<float>(
                new SpriteAnimSys(),
                new RenderingSys(spriteBatch)
            );

            particleSeqSys = new SequentialSystem<float>(
                new ExplosionAnimSys(
                    Content.Load<Texture2D>("Sprites/particles-explosion")
                ),
                new ParticleSpawnerSys(),
                new ParticleBeamSys(),
                new ParticleSys(),
                new MarkerFadeSys()
            );

            roundPrepSys = new SequentialSystem<float>(
                new RoundPrepSys(),
                new AttackActionSys(),
                new MarkerUpdateSys(
                    Content.Load<Texture2D>("Sprites/target-player"),
                    Content.Load<Texture2D>("Sprites/target-enemy-danger"),
                    Content.Load<Texture2D>("Sprites/target-enemy-warning")
                )
            );

            playerInputSys = new SequentialSystem<float>(
                new InputSys()
            );

            AISys = new SequentialSystem<float>(
                new AISys()
            );

            simulateSys = new SequentialSystem<float>(
                new TargetDelaySys(),
                new DamageHereSys(),
                new MoveActionSys(),
                new PowerUpSys(),
                new EndSimSys()
            );

            visualiserSys = new SequentialSystem<float>(
                new MoverSys(),
                new ParallaxSys(),
                new TargetHighlightingSys(
                    Content.Load<Texture2D>("Sprites/target-player"),
                    Content.Load<Texture2D>("Sprites/target-enemy-danger"),
                    Content.Load<Texture2D>("Sprites/target-enemy-warning")
                ),
                // this should go last
                new EndVisualiseStateSys()
            );
        }

        private void CreateParallax()
        {
            var bgTexture1 = Config.Textures["bg-stars-1"];
            var background1 = Game1.world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 1.2f });
            background1.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture1,
                Transparency = 0.3f,
                Depth = 0.0003f
            });
            background1.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture1.Width, Config.ScreenHeight + bgTexture1.Height)
            });

            var bgTexture2 = Config.Textures["bg-stars-2"];
            var background2 = Game1.world.CreateEntity();
            background2.Set(new CParallaxBackground { ScrollVelocity = 1.5f });
            background2.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture2,
                Transparency = 0.1f,
                Depth = 0.0004f
            });
            background2.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture2.Width, Config.ScreenHeight + bgTexture2.Height)
            });

            var bgTexture3 = Config.Textures["bg-stars-3"];
            var background3 = Game1.world.CreateEntity();
            background3.Set(new CParallaxBackground { ScrollVelocity = 0.6f });
            background3.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture3,
                Transparency = 0.7f,
                Depth = 0.0001f
            });
            background3.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture3.Width, Config.ScreenHeight + bgTexture3.Height)
            });

            var bgTexture4 = Config.Textures["bg-stars-4"];
            var background4 = Game1.world.CreateEntity();
            background4.Set(new CParallaxBackground { ScrollVelocity = 0.9f });
            background4.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture4,
                Transparency = 0.5f,
                Depth = 0.0002f
            });
            background4.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + bgTexture4.Width, Config.ScreenHeight + bgTexture4.Height)
            });

            var gridTexture = Config.Textures["grid"];
            var grid = Game1.world.CreateEntity();
            grid.Set(new CParallaxBackground
            {
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

            if (Keyboard.GetState().IsKeyDown(Keys.J))
               GameState = GameState.Starting;

            if (GameState == GameState.Starting)
            {
                world.Dispose();
                world = new World();

                _seed = new Random().Next();
                Console.WriteLine($"seed: {_seed}");

                CreateSystems();
                CreateParallax();
                new MapGenerator(_seed, Content.Load<Texture2D>("Sprites/particles-rainbow")).Generate();

                GameState = GameState.Playing;
            }
            else if (GameState == GameState.Playing)
            {
                particleSeqSys.Update(0.0f);
                while (true)
                {
                    currentRoundState = currentRoundState % roundOrder.Length;
                    int startingState = currentRoundState;

                    switch (roundOrder[currentRoundState])
                    {
                        case RoundState.PostTurn:
                            roundPrepSys.Update(0.0f);
                            currentRoundState++;
                            break;
                        case RoundState.Player:
                            playerInputSys.Update(0.0f);
                            break;
                        case RoundState.AI:
                            AISys.Update(0.0f);
                            currentRoundState++;
                            break;
                        case RoundState.Simulate:
                            simulateSys.Update(0.0f);
                            break;
                        case RoundState.TurnVisualiser:
                            visualiserSys.Update(0.0f);
                            break;
                    }

                    if (currentRoundState == startingState || startingState == roundOrder.Length - 1)
                        break;
                }
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