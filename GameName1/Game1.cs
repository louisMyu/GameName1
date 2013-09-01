using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace GameName1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public static int GameWidth;
        public static int GameHeight;
        
        Player m_Player;
        List<Zombie> m_Zombies = new List<Zombie>();
        List<GameObject> m_AllObjects = new List<GameObject>();
        public static bool ZombiesSpawned = false;
        private PowerUp m_PowerUp;
        
        public static double GameTimer = 0;
        public static double ZombieTimer = 0;
        private double ZombieSpawnTimer = 6;
        public static bool itemMade = false;
        public static Random ZombieRandom = new Random(424242);

        Line testLine = new Line(new Vector2(4, 0), new Vector2(0, 4));
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
            Vector2 playerPosition = new Vector2(GameWidth / 2, GameHeight / 2);
            m_Player.Init(Content, playerPosition);
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
            if (GamePad.GetState(0).Buttons.Back == ButtonState.Pressed)
            {
                m_AllObjects.Clear();
                this.Exit();
            }

            // TODO: Add your update logic here
            GameWidth = GraphicsDevice.Viewport.Width;
            GameHeight = GraphicsDevice.Viewport.Height;
            GameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            ZombieTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //check if a game reset or zombie hit and save state and do the action here,
            //so that the game will draw the zombie intersecting the player
            List<GameObject> toRemove = new List<GameObject>();
            bool b = false;
            m_Player.CheckCollisions(m_AllObjects, toRemove, out b);
            if (b) ResetGame();
            foreach (GameObject g in toRemove)
            {
                if (g is Zombie && g != null)
                {
                    m_Zombies.Remove((Zombie)g);
                }
            }
            //may need to do stuff with removed items before clearing it here
            toRemove.Clear();

            if (ZombieTimer >= ZombieSpawnTimer)
            {
                SpawnZombie();
                ZombieTimer = 0;
                if (ZombieSpawnTimer > 1) 
                {
                    ZombieSpawnTimer -= 0.95;
                }
                if (ZombieSpawnTimer <= 0)
                {
                    ZombieSpawnTimer = 0.4;
                }
            }
            if (GameTimer >= 10 && !itemMade)
            {
                MakeItem();
                itemMade = true;
            }
            Vector2 vec = new Vector2();
            Input.ProcessTouchInput(out vec);
            m_Player.ProcessInput(vec);
            m_Player.Update();
            Vector2 playerPos = new Vector2(m_Player.Position.X, m_Player.Position.Y);
            foreach (Zombie z in m_Zombies)
            {
                z.Move(playerPos);
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

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            m_Player.Draw(_spriteBatch);
            foreach (Zombie z in m_Zombies)
            {
                z.Draw(_spriteBatch);
            }
            foreach (GameObject g in m_AllObjects)
            {
                g.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void SpawnZombie()
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
                if (distanceFromPlayer.LengthSquared() >= (150.0f*150f))
                {
                    nearPlayer = false;
                }
            }
            Zombie z = new Zombie();
            z.Position.X = x;
            z.Position.Y = y;
            z.LoadContent(Content);
            m_Zombies.Add(z);
            m_AllObjects.Add(z);
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
            m_PowerUp.Position.X = x;
            m_PowerUp.Position.Y = y;
            m_PowerUp.LoadContent(Content);
            m_AllObjects.Add(m_PowerUp);
        }

        private void ResetGame()
        {
            m_AllObjects.Clear();
            m_Zombies.Clear();
            GameTimer = 0;
            ZombieSpawnTimer = 6;
            ZombieTimer = 0;
            itemMade = false;
        }
    }
}
