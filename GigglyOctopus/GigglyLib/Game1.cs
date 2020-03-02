using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Systems;
using GigglyLib.Components;
using System;
using System.Collections.Generic;

namespace GigglyLib
{
    public enum RoundState
    {
        PreTurn,
        Player,
        AI,
        Simulate,
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
        SequentialSystem<float> roundPrepSys;

        Entity _player;

        public static int currentRoundState = 0;
        public static RoundState[] roundOrder = new RoundState[]
        {
            RoundState.PreTurn,
            RoundState.AI,
            RoundState.Player,
            RoundState.Simulate,
            RoundState.TurnVisualiser,
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

            _player = world.CreateEntity();
            _player.Set(new CPlayer());
            _player.Set(new CGridPosition());
            _player.Set(new CMovable());
            _player.Set(new CSprite { 
                Texture = Content.Load<Texture2D>("Sprites/player"), 
                Transparency = 0.1f, 
                Depth = 0.5f  
            });
            _player.Set(new CWeapon { 
                Damage = 1, 
                CooldownMax = 5, 
                RangeFront = 4, 
                RangeLeft = 4, 
                RangeRight = 4,
                RangeBack = 4,
                AttackPattern = new List<string>
                    {
                        " 1 ",
                        "111",
                        " 1 "
                    }
        });

            var bgTexture1 = Content.Load<Texture2D>("Sprites/bg-stars-1");
            var background1 = world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 1.2f });
            background1.Set(new CSprite
            {
                X = -Config.ScreenWidth / 2,
                Y = -Config.ScreenHeight / 2,
                Texture = bgTexture1,
                Transparency = 0.3f,
                Depth = 0.0003f
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
                Transparency = 0.1f,
                Depth = 0.0004f
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
                Transparency = 0.7f,
                Depth = 0.0001f
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
                Transparency = 0.5f,
                Depth = 0.0002f
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
            enemy.Set(new CGridPosition { X = 10, Y = 10, Facing = Direction.WEST });
            enemy.Set(new CMovable());
            enemy.Set(new CHealth { Max = 3 });
            //enemy.Set(new CWeapon { Damage = 1, RangeFront = 4, RangeLeft = 1, RangeRight = 1 });
            enemy.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/enemy"), Depth = 0.25f, X = 10 * Config.TileSize, Y = 10 * Config.TileSize, });

            var enemy2 = world.CreateEntity();
            enemy2.Set(new CEnemy());
            enemy2.Set(new CGridPosition { X = 11, Y = 10, Facing = Direction.WEST });
            enemy2.Set(new CMovable());
            enemy2.Set(new CHealth { Max = 3 });
            //enemy2.Set(new CWeapon { Damage = 1, RangeFront = 4, RangeLeft = 1, RangeRight = 1 });
            enemy2.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/enemy"), Depth = 0.25f, X = 11 * Config.TileSize, Y = 10 * Config.TileSize, });

            var enemy3 = world.CreateEntity();
            enemy3.Set(new CEnemy());
            enemy3.Set(new CGridPosition { X = 11, Y = 12, Facing = Direction.WEST });
            enemy3.Set(new CMovable());
            enemy3.Set(new CHealth { Max = 3 });
            //enemy3.Set(new CWeapon { Damage = 1, RangeFront = 4, RangeLeft = 1, RangeRight = 1 });
            enemy3.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/enemy"), Depth = 0.25f, X = 11 * Config.TileSize, Y = 12 * Config.TileSize, });

            drawSys = new SequentialSystem<float>(
                new SpriteAnimSys(world),
                new RenderingSys(world, spriteBatch)
            );

            particleSeqSys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world)
            );

            roundPrepSys = new SequentialSystem<float>(
                new RoundPrepSys(world),
                new AttackActionSys(world),
                new MarkerSpawnerSys(world, 
                    Content.Load<Texture2D>("Sprites/target-player"),
                    Content.Load<Texture2D>("Sprites/target-enemy-danger"),
                    Content.Load<Texture2D>("Sprites/target-enemy-warning")
                )
            );

            playerInputSys = new SequentialSystem<float>(
                new InputSys(world)
            );

            AISys = new SequentialSystem<float>(
                new AISys(world)
            );

            simulateSys = new SequentialSystem<float>(
                new UpdateTargetAnimTypeSys(world),
                new TargetDelaySys(world),
                new DamageHereSys(world),
                new MoveActionSys(world),
                new EndSimSys(world)
            );

            visualiserSys = new SequentialSystem<float>(
                new MoverSys(world),
                new ParallaxSys(world, _player),
                new TargetHighlightingSys(
                    world,
                    Content.Load<Texture2D>("Sprites/target-player"),
                    Content.Load<Texture2D>("Sprites/target-enemy-danger"),
                    Content.Load<Texture2D>("Sprites/target-enemy-warning")
                ),
                // this should go last
                new EndVisualiseStateSys(world)
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

            while (true)
            {
                currentRoundState = currentRoundState % roundOrder.Length;
                int startingState = currentRoundState;

                switch (roundOrder[currentRoundState])
                {
                    case RoundState.PreTurn:
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