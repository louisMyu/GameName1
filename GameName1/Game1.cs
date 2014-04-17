using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Dynamics;
using FarseerPhysics;
using FarseerPhysics.Factories;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;

namespace GameName1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        Song m_song;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public static int GameWidth;
        public static int GameHeight;

        public static World m_World;
        
        public Player m_Player;

        private List<GameObject> m_AllObjects = new List<GameObject>();
        public ObjectManager GlobalObjectManager;

        private Menu m_Menu = new Menu();

        public static bool ZombiesSpawned = false;        
        public static double GameTimer = 0;
        
        private UI UserInterface = new UI();
        public int FrameCounter = 0;
        public double elapsedTime = 0;
        Line testLine = new Line(new Vector2(4, 0), new Vector2(0, 4));
        public static Random ZombieRandom = new Random(424242);

        GameState CurrentGameState = GameState.Playing;

        public int NumZombies = 0;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_Player = new Player();
            GlobalObjectManager = new ObjectManager();

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
            GameWidth = GraphicsDevice.Viewport.Width;
            GameHeight = GraphicsDevice.Viewport.Height;
            
            m_World = new World(new Vector2(0, 0));
            ConvertUnits.SetDisplayUnitToSimUnitRatio(5);

            Player p = Player.Load(Content);
            if (p == null)
            {
                Vector2 playerPosition = new Vector2(GameWidth / 2, GameHeight / 2);
                m_Player.Init(Content, playerPosition);
            }
            else
            {
                m_Player = p;
            }
            //init object manager and set objects for it
            GlobalObjectManager.Init(m_Player, Content, m_World);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureBank.SetContentManager(Content);
            m_Player.LoadContent(m_World);
            UserInterface.LoadContent(Content, GameWidth, GameHeight);
            m_Menu.LoadContent(Content);
            GlobalObjectManager.LoadContent();

            m_song = Content.Load<Song>("AuraQualic - DATA (FL Studio Remix)");
            // TODO: use this.Content to load your game content here
            if (Input.UseAccelerometer)
            {
                Input.accelerometer.Start();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            ObjectManager.AllGameObjects.Clear();
            if (Input.UseAccelerometer)
            {
                Input.accelerometer.Stop();
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (CurrentGameState == GameState.Playing)
            {
                if (GamePad.GetState(0).Buttons.Back == ButtonState.Pressed)
                {
                    //m_AllObjects.Clear();
                    //this.Exit();
                    CurrentGameState = GameState.Menu;
                    m_Menu.State = MenuState.Main;
                }
            }
            Input.ProcessTouchInput();
            switch (CurrentGameState) {
                case GameState.Playing:
                    // TODO: Add your update logic here
                    GameWidth = GraphicsDevice.Viewport.Width;
                    GameHeight = GraphicsDevice.Viewport.Height;
                    GameTimer += gameTime.ElapsedGameTime.TotalSeconds;

                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                    ++FrameCounter;
                    if (elapsedTime > 1)
                    {
                        UserInterface.Update(m_Player, FrameCounter);
                        FrameCounter = 0;
                        elapsedTime = 0;
                    }
                    UserInterface.ProcessInput(m_Player);

                    //check if a game reset or zombie hit and save state and do the action here,
                    //so that the game will draw the zombie intersecting the player
                    m_Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    foreach (GameObject g in ObjectManager.AllGameObjects)
                    {
                        g.Update(m_Player);
                    }
                    bool b = false;
                    m_Player.CheckCollisions(out b, m_World);
                    if (b) ResetGame();

                    //cleanup dead objects
                    GlobalObjectManager.Update();

                    m_World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.002f);
                    break;
                case GameState.Menu:
                    bool toQuit;
                    CurrentGameState = m_Menu.Update(out toQuit);
                    if (toQuit)
                    {
                        m_Player.Save();
                        Storage.Save<List<GameObject>>("GameObjects", "ObjectList.dat", ObjectManager.AllGameObjects);
                        ObjectManager.AllGameObjects.Clear();
                        this.Exit();
                    }
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
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch (CurrentGameState) {
                case GameState.Playing:
                    // TODO: Add your drawing code here
                    _spriteBatch.Begin();
                    UserInterface.DrawBackground(_spriteBatch);
                    GlobalObjectManager.Draw(_spriteBatch);
                    m_Player.Draw(_spriteBatch);
                    UserInterface.Draw(_spriteBatch, m_Player);
                    _spriteBatch.End();
                    break;
                case GameState.Menu:
                    _spriteBatch.Begin();
                    m_Menu.Draw(_spriteBatch, m_Player);
                    _spriteBatch.End();
                    break;
            }
            base.Draw(gameTime);
        }

        private void ResetGame()
        {
            ObjectManager.AllGameObjects.Clear();
            GlobalObjectManager.ResetGame();
            
        }
        private void SpawnFace()
        {
            bool nearPlayer = true;
            int x = 0;
            int y = 0;
            while (nearPlayer)
            {
                x = ZombieRandom.Next(GameWidth);
                y = ZombieRandom.Next(GameHeight);

                //don't spawn near player
                Vector2 distanceFromPlayer = new Vector2(x - m_Player.Position.X, y - m_Player.Position.Y);
                if (distanceFromPlayer.LengthSquared() >= (150.0f * 150f))
                {
                    nearPlayer = false;
                }
            }
            Anubis z = new Anubis();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            z.Position = temp;
            z.LoadContent(m_World);
            ObjectManager.AllGameObjects.Add(z);
        }
    }
}
