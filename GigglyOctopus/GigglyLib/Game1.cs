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
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace GigglyLib
{
    public enum GameState 
    {
        Starting,
        Playing,
        GameOver,
        Respawning
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
        public static Random GameStateRandom;
        public static Random NonDeterministicRandom;

        public static string DebugOutput = "";

        // The byte we are currently on in the replay
        public static int ReplayCounter = 5;
        // The part of the byte we are currently on in the replay
        public static int ReplayIntraByteCounter = 0;
        public static bool StoppedLoadingReplay = true;
        public static bool StoppedExportingReplay = true;
        /// <summary>
        /// First 4 bytes are i32 for seed
        /// 5th byte contains what ReplayIntraByteCounter was at end of replay
        /// then each byte contains 4 moves per byte, ordered like this: 44332211
        /// </summary>
        public static List<byte> ReplayData = new List<byte>(1024);
        public static bool IsReplayMode = false;

        public static CWeaponsArray startingWeapons = new CWeaponsArray()
        {
            Weapons = new List<CWeapon> {
            }
        };

        private static int _seed;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static World world = new World();

        public static bool playExplosion = false;
        public static bool warningStop = false;
        public static int stageCount = 0;

        Song BGM;

        SequentialSystem<float> simulateSys;
        SequentialSystem<float> playerInputSys;
        SequentialSystem<float> visualiserSys;
        SequentialSystem<float> AISys;
        SequentialSystem<float> particleSeqSys;
        SequentialSystem<float> drawSys;
        SequentialSystem<float> postTurnSys;

        public static HashSet<(int,int)> Tiles;
        public static int[,] CostGrid;
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
            Window.Title = "PulseEXE";
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Config.ScreenWidth;
            graphics.PreferredBackBufferHeight = Config.ScreenHeight;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            graphics.ApplyChanges();
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
            Config.Textures.Add("portal", Content.Load<Texture2D>("Sprites/portal"));

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
            Config.Textures.Add("blank", Content.Load<Texture2D>("Sprites/blank"));

            // Audio
            BGM = Content.Load<Song>("Sounds/background-music");
            Config.SFX.Add("laser-basic", Content.Load<SoundEffect>("Sounds/laser-basic"));
            Config.SFX.Add("enemy-hit", Content.Load<SoundEffect>("Sounds/enemy-hit"));
            Config.SFX.Add("enemy-destroyed", Content.Load<SoundEffect>("Sounds/enemy-destroyed"));
            Config.SFX.Add("player-heal", Content.Load<SoundEffect>("Sounds/player-heal"));
            Config.SFX.Add("player-upgrade", Content.Load<SoundEffect>("Sounds/player-upgrade"));
            Config.SFX.Add("player-move", Content.Load<SoundEffect>("Sounds/player-move"));
            Config.SFX.Add("player-death", Content.Load<SoundEffect>("Sounds/player-death"));
            Config.SFX.Add("player-hit", Content.Load<SoundEffect>("Sounds/player-hit"));
            Config.SFX.Add("game-over", Content.Load<SoundEffect>("Sounds/game-over"));
            Config.SFX.Add("danger", Content.Load<SoundEffect>("Sounds/danger"));
            Config.SFX.Add("warning", Content.Load<SoundEffect>("Sounds/warning"));
            Config.SFX.Add("explosion", Content.Load<SoundEffect>("Sounds/explosion"));
            Config.SFX.Add("charging", Content.Load<SoundEffect>("Sounds/charging"));
        }

        private void CreateSystems()
        {
            drawSys = new SequentialSystem<float>(
                new SpriteAnimSys(),
                new RenderingSys(spriteBatch),
                new ParticleRenderSys(spriteBatch)
            );

            particleSeqSys = new SequentialSystem<float>(
                new ExplosionAnimSys(),
                new ParticleSpawnerSys(),
                new ParticleBeamSys(),
                new ParticleSys(),
                new MarkerFadeSys(),
                new PowerUpAnimSys(),
                new FadeInSys()
            );

            AISys = new SequentialSystem<float>(
                new AISys()
            );

            playerInputSys = !IsReplayMode ?
                new SequentialSystem<float>(
                    new InputSys(),
                    new InputRecorder()
                ) :
                new SequentialSystem<float>(
                    new ReplayInputSys()
                );

            simulateSys = new SequentialSystem<float>(
                new TargetDelaySys(),
                new DamageHereSys(),
                new MoveActionSys(),
                new PowerUpSys(),
                new PortalSys(),
                new EndSimSys()
            );

            postTurnSys = new SequentialSystem<float>(
                new RoundPrepSys(),
                new AttackActionSys(),
                new MarkerUpdateSys()
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

            var fadeIn = Game1.world.CreateEntity();
            fadeIn.Set(new CSprite
            {
                X = X,
                Y = Y,
                Texture = "particles-rainbow",
                Depth = 1
            });
            fadeIn.Set(new CScalable{ Scale = 500 });
            fadeIn.Set<CFadeIn>();
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
            var kS = Keyboard.GetState();
            /*if (kS.IsKeyDown(Keys.LeftShift) && kS.IsKeyDown(Keys.D))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
                StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(DebugOutput);
                streamWriter.Close();
            }*/

            if ((kS.IsKeyDown(Keys.LeftControl) || kS.IsKeyDown(Keys.RightControl)) && kS.IsKeyDown(Keys.S))
            {
                if (StoppedExportingReplay)
                    ExportReplayData();
                StoppedExportingReplay = false;
            }
            if (!kS.IsKeyDown(Keys.LeftControl) && !kS.IsKeyDown(Keys.RightControl) && !kS.IsKeyDown(Keys.S))
            {
                StoppedExportingReplay = true;
            }

            if ((kS.IsKeyDown(Keys.LeftControl) || kS.IsKeyDown(Keys.RightControl)) && kS.IsKeyDown(Keys.L))
            {
                if (StoppedLoadingReplay)
                {
                    world.Dispose();
                    IsReplayMode = true;
                    startingWeapons = new CWeaponsArray { Weapons = new List<CWeapon>() };
                    ReplayData = new List<byte>();
                    ReplayIntraByteCounter = 0;
                    ReplayCounter = 5;
                    playExplosion = false;
                    warningStop = false;
                    currentRoundState = 0;
                    stageCount = 0;
                    ParticleManager.EndIndex = 0;
                    ParticleManager.Particles = new Particle[ParticleManager.Particles.Length];
                    GameState = GameState.Starting;
                }
                StoppedLoadingReplay = false;
            }
            if (!kS.IsKeyDown(Keys.LeftControl) && !kS.IsKeyDown(Keys.RightControl) && !kS.IsKeyDown(Keys.L))
            {
                StoppedLoadingReplay = true;
            }

            if (GameState == GameState.Starting)
            {
                world.Dispose();
                world = new World();
                stageCount++;

                if (ReplayData.Count == 0)
                {
                    if (IsReplayMode)
                    {
                        // Read data in from replay file
                        Directory.CreateDirectory("./Replays");
                        string[] files = Directory.GetFiles("./Replays");
                        int suffix = -1;
                        for (int i = 0; i < files.Length; i++)
                        {
                            string name = files[i];
                            name = name.Split('-')[1];
                            name = name.Split('.')[0];
                            int number = int.Parse(name);
                            if (number > suffix)
                                suffix = number;
                        }

                        if (suffix == -1)
                            throw new Exception("Could not find replay file");
                        var fs = new FileStream("./Replays/Replay-" + suffix.ToString() + ".pulsereplay", FileMode.Open, FileAccess.Read);
                        byte[] data = new byte[(int)fs.Length];
                        fs.Read(data, 0, (int)fs.Length);
                        ReplayData = new List<byte>(data);

                        _seed = BitConverter.ToInt32(data, 0);
                        Game1.GameStateRandom = new Random(_seed);
                        Game1.NonDeterministicRandom = new Random(_seed);

                        Console.WriteLine($"seed: {_seed}");
                        DebugOutput += "SEED: " + _seed.ToString();
                    }
                    else
                    {
                        _seed = new Random().Next();
                        Game1.GameStateRandom = new Random(_seed);
                        Game1.NonDeterministicRandom = new Random(_seed);

                        foreach (var data in BitConverter.GetBytes(_seed))
                        {
                            ReplayData.Add(data);
                        }
                        // 5th byte
                        ReplayData.Add(0);
                        // The byte for actual data storage
                        ReplayData.Add(0);

                        Console.WriteLine($"seed: {_seed}");
                        DebugOutput += "SEED: " + _seed.ToString();
                    }
                }



                CreateSystems();
                new MapGenerator().Generate();
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
                        case RoundState.AI:
                            AISys.Update(0.0f);
                            currentRoundState++;
                            break;
                        case RoundState.Player:
                            playerInputSys.Update(0.0f);
                            break;
                        case RoundState.Simulate:
                            simulateSys.Update(0.0f);
                            break;
                        case RoundState.PostTurn:
                            postTurnSys.Update(0.0f);
                            currentRoundState++;
                            break;
                        case RoundState.TurnVisualiser:
                            visualiserSys.Update(0.0f);
                            break;
                    }

                    if (currentRoundState == startingState || startingState == roundOrder.Length - 1)
                        break;
                }
            }
            else if (GameState == GameState.GameOver)
            {
                stageCount = 0;
                if (!IsReplayMode)
                    ExportReplayData();

                MediaPlayer.Stop();
                var targetBuilder = world.GetEntities().Without<CPlayer>().Without<CParallaxBackground>().WithEither<CSprite>().Or<CParticleSpawner>();
                var toClear = targetBuilder.AsSet().GetEntities();
                foreach (var e in toClear)
                {
                    e.Remove<CParticleSpawner>();
                    ref var s = ref e.Get<CSprite>();
                    s.Texture = "blank";
                }

                Config.SFX["player-death"].Play();

                Player.Remove<CPlayer>();
                Player.Remove<CHealth>();
                Player.Remove<CParticleSpawner>();
                Player.Set(new CScalable { 
                    Scale = 1.0f
                });

                Config.SFX["game-over"].Play();
                ref var sprite = ref Player.Get<CSprite>();
                for (int i = 0; i < 18; ++i)
                {
                    int times = 10;
                    while(times --> 0)
                    {
                        ParticleManager.CreateParticle(
                            x: sprite.X,
                            y: sprite.Y,
                            texture: PARTICLES[i],
                            deltaRotation: Game1.NonDeterministicRandom.NextFloat() * 0.0f,
                            velocity: Game1.NonDeterministicRandom.NextFloat() * 0.1f + 0.1f,
                            scale: (Game1.NonDeterministicRandom.NextFloat() * 0.4f) + 0.3f,
                            depth: 0.3f,
                            transparency: (Game1.NonDeterministicRandom.NextFloat() * 0.2f) + 0.12f,
                            rotation: Game1.NonDeterministicRandom.NextFloat() * 2 * (float)Math.PI
                            );
                    }
                }

                GameState = GameState.Respawning;
            }
            else if(GameState == GameState.Respawning)
            {
                ref var sprite = ref Player.Get<CSprite>();
                ref var scale = ref Player.Get<CScalable>();
                if (sprite.Transparency < 1 && !Player.Has<CPlayer>())
                {
                    sprite.Transparency += 0.1f;
                    scale.Scale *= 1.1f;
                }
                else if (!Player.Has<CPlayer>())
                {
                    sprite.Rotation = 0;
                    Player.Set<CPlayer>();
                }
                else if (scale.Scale > 1.0f && sprite.Texture != "particles-explosion")
                {
                    scale.Scale /= 1.005f;
                }
                else if (!Player.Has<CCharging>())
                {
                    Config.SFX["charging"].Play();
                    Player.Set<CCharging>();
                }
                else
                {
                    var beamAnim = Game1.world.CreateEntity();
                    beamAnim.Set(new CParticleBeam
                    {
                        SourceX = Player.Get<CGridPosition>().X + Game1.NonDeterministicRandom.Next(27) - 13,
                        SourceY = Player.Get<CGridPosition>().Y + Game1.NonDeterministicRandom.Next(15) - 7,
                        DestX = Player.Get<CGridPosition>().X,
                        DestY = Player.Get<CGridPosition>().Y,
                        RandomColours = true
                    });
                    sprite.Texture = "particles-explosion";
                    scale.Scale *= 1.1f;
                    sprite.Transparency -= 0.01f;
                    if (sprite.Transparency < 0f)
                        GameState = GameState.Starting;
                }
                particleSeqSys.Update(0.0f);
            }

            if(playExplosion)
            {
                Config.SFX["explosion"].Play();
                playExplosion = false;
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

        private void ExportReplayData() 
        {
            Directory.CreateDirectory("./Replays");
            string[] files = Directory.GetFiles("./Replays");
            int suffix = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string name = files[i];
                name = name.Split('-')[1];
                name = name.Split('.')[0];
                int number = int.Parse(name);
                if (number >= suffix)
                    suffix = number + 1;
            }
            var fs = new FileStream("./Replays/Replay-" + suffix.ToString() + ".pulsereplay", FileMode.Create, FileAccess.Write);
            var data = ReplayData.ToArray();
            data[4] = (byte)ReplayIntraByteCounter;
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
    }
}