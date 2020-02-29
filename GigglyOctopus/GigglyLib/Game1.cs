using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Systems;
using GigglyLib.Components;

namespace GigglyLib
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World world = new World();
        SequentialSystem<float> updateSys;
        SequentialSystem<float> drawSys;

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

            var background1 = world.CreateEntity();
            background1.Set(new CParallaxBackground { ScrollVelocity = 1f });
            background1.Set(new CSprite { 
                X = 225,
                Y = 225,
                Texture = Content.Load<Texture2D>("Sprites/bg-stars-1")
            });
            background1.Set(new CSourceRectangle { 
                Rectangle = new Rectangle(0, 0, Config.ScreenWidth + 450, Config.ScreenHeight + 450) 
            });

            updateSys = new SequentialSystem<float>(
                new InputSys(world),
                new ThrusterSys(world, Content.Load<Texture2D>("Sprites/particles-star")),
                new ParticleSys(world),
                new GridTransformSys(world),
                new MoverSys(world),
                new ParallaxSys(world, player)
            );

            drawSys = new SequentialSystem<float>(
                new RenderingSys(world, spriteBatch)
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

            updateSys.Update(0.0f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(15, 15, 15));
            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.LinearWrap, null, null);
            drawSys.Update(0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}