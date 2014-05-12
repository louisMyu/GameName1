#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using GameName1;
using FarseerPhysics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace GameName1
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        public static World m_World;
        public Player m_Player;
        public ObjectManager GlobalObjectManager;
        private Menu m_Menu = new Menu();
        private UI UserInterface = new UI();
        public bool SlowMotion = false;
        public static TimeSpan TimeToDeath = TimeSpan.FromSeconds(30);
        public static Random ZombieRandom = new Random(424242);
        private TouchCollection TouchesCollected;
        Song m_song;
        Random random = new Random();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            m_Player = new Player();
            GlobalObjectManager = new ObjectManager();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = ScreenManager.Game.Content;

            gameFont = content.Load<SpriteFont>("GSMgamefont");

            m_World = new World(new Vector2(0, 0));
            ConvertUnits.SetDisplayUnitToSimUnitRatio(5);

            Player p = Player.Load(content);
            if (p == null)
            {
                Vector2 playerPosition = new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2);
                m_Player.Init(content, playerPosition);
            }
            else
            {
                m_Player = p;
            }
            //init object manager and set objects for it
            GlobalObjectManager.Init(m_Player, content, m_World);


            TextureBank.SetContentManager(content);
            SoundBank.SetContentManager(content);
            m_Player.LoadContent(m_World);
            UserInterface.LoadContent(content, Game1.GameWidth, Game1.GameHeight);
            m_Menu.LoadContent(content);
            GlobalObjectManager.LoadContent();

            m_song = SoundBank.GetSong("AuraQualic - DATA (FL Studio Remix)");

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                try
                {
                    TimeSpan customElapsedTime = gameTime.ElapsedGameTime;
                    if (SlowMotion)
                    {
                        customElapsedTime = new TimeSpan((long)(customElapsedTime.Ticks * 0.5));
                    }

                    if (TimeToDeath <= TimeSpan.FromSeconds(0))
                    {
                        //SlowMotion = true;
                        ResetGame();
                    }
                    TimeToDeath -= gameTime.ElapsedGameTime;
                    // TODO: Add your update logic here
                    UserInterface.ProcessInput(m_Player, TouchesCollected);
                    UserInterface.Update(TimeToDeath, customElapsedTime);
                    //check if a game reset or zombie hit and save state and do the action here,
                    //so that the game will draw the zombie intersecting the player
                    m_Player.Update(customElapsedTime);
                    foreach (GameObject g in ObjectManager.AllGameObjects)
                    {
                        g.Update(m_Player, customElapsedTime);
                    }
                    bool b = false;
                    m_Player.CheckCollisions(out b, m_World);
                    if (b) ResetGame();

                    //cleanup dead objects
                    GlobalObjectManager.Update(customElapsedTime);

                    m_World.Step((float)customElapsedTime.TotalMilliseconds * 0.002f);
                }
                catch (Exception e)
                {
                }
                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }
        private void ResetGame()
        {
            ObjectManager.ClearGrid();
            ObjectManager.AllGameObjects.Clear();
            GlobalObjectManager.ResetGame();
            TimeToDeath = TimeSpan.FromSeconds(40);
        }
        private void SpawnFace()
        {
            bool nearPlayer = true;
            int x = 0;
            int y = 0;
            while (nearPlayer)
            {
                x = ZombieRandom.Next(Game1.GameWidth);
                y = ZombieRandom.Next(Game1.GameHeight);

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

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(Input input)
        {
            TouchesCollected = input.TouchState;
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch _spriteBatch = ScreenManager.SpriteBatch;

            _spriteBatch.Begin();
            try
            {
                UserInterface.DrawBackground(_spriteBatch);
                GlobalObjectManager.Draw(_spriteBatch);
                m_Player.Draw(_spriteBatch);
                UserInterface.Draw(_spriteBatch, m_Player);
            }
            catch (Exception e)
            {
            }
            _spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion
    }
}
