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
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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
            player.Set(new CSprite { Texture = Content.Load<Texture2D>("Sprites/player"), Depth = 1 });

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
                new InputSys(world)
            );

            actionSys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                new GridTransformSys(world),
                new MoverSys(world),
                new EndActionStateSys(world)
            );

            AISys = new SequentialSystem<float>(
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                //new GridTransformSys(world),
                new AISys(world)
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

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
            drawSys.Update(0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}