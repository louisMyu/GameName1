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
        private Menu m_Menu = new Menu();

        public static bool ZombiesSpawned = false;
        private PowerUp m_PowerUp;
        
        public static double GameTimer = 0;
        public static double ZombieTimer = 0;
        private double ZombieSpawnTimer = 50;
        private int MaxZombies = 50;

        public static bool itemMade = false;
        public static bool face = false;
        public static Random ZombieRandom = new Random(424242);
        private UI UserInterface = new UI();
        public int FrameCounter = 0;
        public double elapsedTime = 0;
        Line testLine = new Line(new Vector2(4, 0), new Vector2(0, 4));

        GameState CurrentGameState = GameState.Playing;

        public int NumZombies = 0;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_Player = new Player();
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
            m_AllObjects = Storage.Load<List<GameObject>>("GameObjects", "ObjectList.dat");
            if (m_AllObjects == null)
            {
                m_AllObjects = new List<GameObject>();
            }
            else
            {
                foreach (GameObject g in m_AllObjects)
                {
                    g.Load(Content, m_World);
                }
            }
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
            m_Player.LoadContent(Content);
            UserInterface.LoadContent(Content, GameWidth, GameHeight);
            Magic.TextureInit(Content);
            m_Menu.LoadContent(Content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(10);

            m_song = Content.Load<Song>("AuraQualic - DATA (FL Studio Remix)");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            m_AllObjects.Clear();
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
            List<Vector2> vec = new List<Vector2>();
            Input.ProcessTouchInput(out vec);
            switch (CurrentGameState) {
                case GameState.Playing:
                    // TODO: Add your update logic here
                    GameWidth = GraphicsDevice.Viewport.Width;
                    GameHeight = GraphicsDevice.Viewport.Height;
                    GameTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    ++ZombieTimer;

                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                    ++FrameCounter;
                    if (elapsedTime > 1)
                    {
                        UserInterface.Update(m_Player, FrameCounter);
                        FrameCounter = 0;
                        elapsedTime = 0;
                    }
                    UserInterface.ProcessInput(vec, m_Player, m_AllObjects);

                    //check if a game reset or zombie hit and save state and do the action here,
                    //so that the game will draw the zombie intersecting the player
                    m_Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Vector2 playerPos = new Vector2(m_Player.Position.X, m_Player.Position.Y);
                    foreach (GameObject g in m_AllObjects)
                    {
                        g.Update(playerPos);
                    }
                    bool b = false;
                    m_Player.CheckCollisions(m_AllObjects, out b, m_World);
                    if (b) ResetGame();


                    if (ZombieTimer >= ZombieSpawnTimer && NumZombies< MaxZombies)
                    {
                        SpawnZombie();
                        ZombieTimer = 0;
                        if (ZombieSpawnTimer > 10) 
                        {
                            ZombieSpawnTimer -= 1;
                        }
                        if (ZombieSpawnTimer < 10)
                        {
                            ZombieSpawnTimer = 10;
                        }
                    }
                    if (GameTimer >= 10 && !itemMade)
                    {
                        MakeItem();
                        itemMade = true;
                    }
                    if (GameTimer >= 25 && !face)
                    {
                        SpawnFace();
                        face = true;
                    }
                    m_World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.002f);
                    break;
                case GameState.Menu:
                    bool toQuit;
                    CurrentGameState = m_Menu.Update(vec, out toQuit);
                    if (toQuit)
                    {
                        m_Player.Save();
                        Storage.Save<List<GameObject>>("GameObjects", "ObjectList.dat", m_AllObjects);
                        m_AllObjects.Clear();
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
                    m_Player.Draw(_spriteBatch);
                    //foreach (Zombie z in m_Zombies)
                    //{
                    //    z.Draw(_spriteBatch);
                    //}
                    foreach (GameObject g in m_AllObjects)
                    {
                        g.Draw(_spriteBatch);
                    }
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

        private void SpawnZombie()
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
                if (distanceFromPlayer.LengthSquared() >= (150.0f*150f))
                {
                    nearPlayer = false;
                }
            }
            Zombie z = new Zombie();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            z.Position = temp;
            z.LoadContent(Content, m_World);
            m_AllObjects.Add(z);
            ++NumZombies;
        }

        private void MakeItem()
        {
            bool nearPlayer = true;
            int x = 0;
            int y = 0;
            while (nearPlayer)
            {
                x = ZombieRandom.Next(720);
                y = ZombieRandom.Next(1280);

                //don't spawn near player
                Vector2 distanceFromPlayer = new Vector2(x - m_Player.Position.X, y - m_Player.Position.Y);
                if (distanceFromPlayer.LengthSquared() >= (150.0f*150.0f))
                {
                    nearPlayer = false;
                }
            }
            m_PowerUp = new PowerUp();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            m_PowerUp.Position = temp;
            m_PowerUp.LoadContent(Content);
            m_AllObjects.Add(m_PowerUp);
        }

        private void ResetGame()
        {
            m_AllObjects.Clear();
            GameTimer = 0;
            ZombieSpawnTimer = 6;
            ZombieTimer = 0;
            itemMade = false;
            face = false;
            m_Player.Score = 0;
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
            z.LoadContent(Content, m_World);
            m_AllObjects.Add(z);
        }
    }
}
