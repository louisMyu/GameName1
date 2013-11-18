using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

using Microsoft.Devices.Sensors;


namespace FarSeerTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _batch;

        private SpriteFont _font;

        private World _world;

        private Body _circleBody;
        private Body _groundBody;

        private Texture2D _circleSprite;
        private Texture2D _groundSprite;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private Vector2 _groundOrigin;
        private Vector2 _circleOrigin;

        
#if !XBOX360
        const string Text = "Press A or D to rotate the ball\n" +
                            "Press Space to jump\n";
#endif
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;

            Content.RootDirectory = "Content";

            //Create a world with gravity.
            _world = new World(new Vector2(0, 9.82f));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            _font = Content.Load<SpriteFont>("font");

            // Load sprites
            _circleSprite = Content.Load<Texture2D>("CircleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = Content.Load<Texture2D>("GroundSprite"); // 512px x 64px =>   8m x 1m

            /* We need XNA to draw the ground and circle at the center of the shapes */
            _groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);
            _circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(96 / 2f), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _circleBody.Restitution = 0.6f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            if (GamePad.GetState(0).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            List<Vector2> touches = new List<Vector2>();
            Input.ProcessTouchInput(out touches);
            foreach (Vector2 touch in touches)
            {
                _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
            }
            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw circle and ground
            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            _batch.Draw(_circleSprite, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, _circleBody.Rotation, _circleOrigin, 1f, SpriteEffects.None, 0f);
            _batch.Draw(_groundSprite, ConvertUnits.ToDisplayUnits(_groundBody.Position), null, Color.White, 0f, _groundOrigin, 1f, SpriteEffects.None, 0f);
            _batch.End();

            // Display instructions
            _batch.Begin();
            _batch.DrawString(_font, Text, new Vector2(14f, 14f), Color.Black);
            _batch.DrawString(_font, Text, new Vector2(12f, 12f), Color.White);
            _batch.End();

            base.Draw(gameTime);
        }
        public void LeftButton()
        {
            _circleBody.ApplyTorque(-20);
        }
        public void RightButton()
        {
            _circleBody.ApplyTorque(20);
        }
        public void JumpButton()
        {
            _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
        }
        //void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        //{
        //    AccelerometerReading reading = e.SensorReading;
        //    Vector3 acceleration = reading.Acceleration;
        //}
    }
}
