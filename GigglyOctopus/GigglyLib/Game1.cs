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
using Microsoft.Xna.Framework.Media;

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

        Song BGM;

        SequentialSystem<float> simulateSys;
        SequentialSystem<float> playerInputSys;
        SequentialSystem<float> visualiserSys;
        SequentialSystem<float> AISys;
        SequentialSystem<float> particleSeqSys;
        SequentialSystem<float> drawSys;
        SequentialSystem<float> roundPrepSys;

        public static bool[,] Tiles;
        public static Entity Player;
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

        public static List<string> PARTICLES = new List<string> { 
            "particles-red",
            "particles-orange",
            "particles-golden",
            "particles-yellow",
            "particles-lime",
            "particles-lightgreen",
            "particles-green",
            "particles-seagreen",
            "particles-aquamarine",
            "particles-cyan",
            "particles-lightblue",
            "particles-blue",
            "particles-violet",
            "particles-purple",
            "particles-lightpurple",
            "particles-pink",
            "particles-magenta",
            "particles-fuschia",
        };

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

            Config.Textures.Add("enemy-red", Content.Load<Texture2D>("Sprites/enemy-red"));
            Config.Textures.Add("enemy-orange", Content.Load<Texture2D>("Sprites/enemy-orange"));
            Config.Textures.Add("enemy-golden", Content.Load<Texture2D>("Sprites/enemy-golden"));
            Config.Textures.Add("enemy-yellow", Content.Load<Texture2D>("Sprites/enemy-yellow"));
            Config.Textures.Add("enemy-lime", Content.Load<Texture2D>("Sprites/enemy-lime"));
            Config.Textures.Add("enemy-lightgreen", Content.Load<Texture2D>("Sprites/enemy-lightgreen"));
            Config.Textures.Add("enemy-green", Content.Load<Texture2D>("Sprites/enemy-green"));
            Config.Textures.Add("enemy-seagreen", Content.Load<Texture2D>("Sprites/enemy-seagreen"));
            Config.Textures.Add("enemy-aquamarine", Content.Load<Texture2D>("Sprites/enemy-aquamarine"));
            Config.Textures.Add("enemy-cyan", Content.Load<Texture2D>("Sprites/enemy-cyan"));
            Config.Textures.Add("enemy-lightblue", Content.Load<Texture2D>("Sprites/enemy-lightblue"));
            Config.Textures.Add("enemy-blue", Content.Load<Texture2D>("Sprites/enemy-blue"));
            Config.Textures.Add("enemy-violet", Content.Load<Texture2D>("Sprites/enemy-violet"));
            Config.Textures.Add("enemy-purple", Content.Load<Texture2D>("Sprites/enemy-purple"));
            Config.Textures.Add("enemy-lightpurple", Content.Load<Texture2D>("Sprites/enemy-lightpurple"));
            Config.Textures.Add("enemy-pink", Content.Load<Texture2D>("Sprites/enemy-pink"));
            Config.Textures.Add("enemy-magenta", Content.Load<Texture2D>("Sprites/enemy-magenta"));
            Config.Textures.Add("enemy-fuschia", Content.Load<Texture2D>("Sprites/enemy-fuschia"));

            Config.Textures.Add("asteroid", Content.Load<Texture2D>("Sprites/asteroid-tile"));
            Config.Textures.Add("player", Content.Load<Texture2D>("Sprites/player"));
            Config.Textures.Add("grid", Content.Load<Texture2D>("Sprites/grid"));
            Config.Textures.Add("bg-stars-1", Content.Load<Texture2D>("Sprites/bg-stars-1"));
            Config.Textures.Add("bg-stars-2", Content.Load<Texture2D>("Sprites/bg-stars-2"));
            Config.Textures.Add("bg-stars-3", Content.Load<Texture2D>("Sprites/bg-stars-3"));
            Config.Textures.Add("bg-stars-4", Content.Load<Texture2D>("Sprites/bg-stars-4"));

            Config.Textures.Add("particles-red", Content.Load<Texture2D>("Sprites/particles-red"));
            Config.Textures.Add("particles-orange", Content.Load<Texture2D>("Sprites/particles-orange"));
            Config.Textures.Add("particles-golden", Content.Load<Texture2D>("Sprites/particles-golden"));
            Config.Textures.Add("particles-yellow", Content.Load<Texture2D>("Sprites/particles-yellow"));
            Config.Textures.Add("particles-lime", Content.Load<Texture2D>("Sprites/particles-lime"));
            Config.Textures.Add("particles-lightgreen", Content.Load<Texture2D>("Sprites/particles-lightgreen"));
            Config.Textures.Add("particles-green", Content.Load<Texture2D>("Sprites/particles-green"));
            Config.Textures.Add("particles-seagreen", Content.Load<Texture2D>("Sprites/particles-seagreen"));
            Config.Textures.Add("particles-aquamarine", Content.Load<Texture2D>("Sprites/particles-aquamarine"));
            Config.Textures.Add("particles-cyan", Content.Load<Texture2D>("Sprites/particles-cyan"));
            Config.Textures.Add("particles-lightblue", Content.Load<Texture2D>("Sprites/particles-lightblue"));
            Config.Textures.Add("particles-blue", Content.Load<Texture2D>("Sprites/particles-blue"));
            Config.Textures.Add("particles-violet", Content.Load<Texture2D>("Sprites/particles-violet"));
            Config.Textures.Add("particles-purple", Content.Load<Texture2D>("Sprites/particles-purple"));
            Config.Textures.Add("particles-lightpurple", Content.Load<Texture2D>("Sprites/particles-lightpurple"));
            Config.Textures.Add("particles-pink", Content.Load<Texture2D>("Sprites/particles-pink"));
            Config.Textures.Add("particles-magenta", Content.Load<Texture2D>("Sprites/particles-magenta"));
            Config.Textures.Add("particles-fuschia", Content.Load<Texture2D>("Sprites/particles-fuschia"));
            Config.Textures.Add("particles-explosion", Content.Load<Texture2D>("Sprites/particles-explosion"));
            Config.Textures.Add("particles-smoke", Content.Load<Texture2D>("Sprites/particles-smoke"));
            Config.Textures.Add("particles-rainbow", Content.Load<Texture2D>("Sprites/particles-rainbow"));
            Config.Textures.Add("power-up", Content.Load<Texture2D>("Sprites/power-up"));
            Config.Textures.Add("target-player", Content.Load<Texture2D>("Sprites/target-player"));
            Config.Textures.Add("target-enemy-danger", Content.Load<Texture2D>("Sprites/target-enemy-danger"));
            Config.Textures.Add("target-enemy-warning",Content.Load<Texture2D>("Sprites/target-enemy-warning"));

            // Audio
            BGM = Content.Load<Song>("Sounds/background-music");
        }

        private void CreateSystems()
        {
            drawSys = new SequentialSystem<float>(
                new SpriteAnimSys(),
                new RenderingSys(spriteBatch)
            );

            particleSeqSys = new SequentialSystem<float>(
                new ExplosionAnimSys(),
                new ParticleSpawnerSys(),
                new ParticleBeamSys(),
                new ParticleSys(),
                new MarkerFadeSys(),
                new PowerUpAnimSys()
            );

            roundPrepSys = new SequentialSystem<float>(
                new RoundPrepSys(),
                new AttackActionSys(),
                new MarkerUpdateSys()
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
                new TargetHighlightingSys(),
                // this should go last
                new EndVisualiseStateSys()
            );
        }

        private void CreateParallax()
        {
            var X = Player.Get<CGridPosition>().X * Config.TileSize;
            var Y = Player.Get<CGridPosition>().Y * Config.TileSize;
            var bgTexture1 = Config.Textures["bg-stars-1"];
            var background1 = Game1.world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 1.2f });
            background1.Set(new CSprite
            {
                Texture = "bg-stars-1",
                Transparency = 0.3f,
                Depth = 0.0003f,
                X = X,
                Y = Y
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
                Texture = "bg-stars-2",
                Transparency = 0.1f,
                Depth = 0.0004f,
                X = X,
                Y = Y
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
                Texture = "bg-stars-3",
                Transparency = 0.7f,
                Depth = 0.0001f,
                X = X,
                Y = Y
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
                Texture = "bg-stars-4",
                Transparency = 0.5f,
                Depth = 0.0002f,
                X = X,
                Y = Y
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
                X = X + 15.5f,
                Y = Y + gridTexture.Height / 2 - 0.5f,
                Texture = "grid"
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
                new MapGenerator(_seed).Generate();
                CreateParallax();
                MediaPlayer.Play(BGM);
                MediaPlayer.IsRepeating = true;

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

            (float x, float y) = (-Player.Get<CSprite>().X + Config.ScreenWidth / 2, -Player.Get<CSprite>().Y + Config.ScreenHeight / 2);
            var matrix = Matrix.CreateTranslation(x, y, 0);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.LinearWrap, null, null, null, matrix);
            drawSys.Update(0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}