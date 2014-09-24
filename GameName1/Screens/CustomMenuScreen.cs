﻿#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace GameName1
{
    class CustomMenuScreen : MenuScreen
    {
        #region Initialization

        private CustomMenuEntry gameMenuEntry;
        private CustomMenuEntry optionMenuEntry;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CustomMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            Rectangle playGameRec = new Rectangle(100, 50, 50, 200);
            gameMenuEntry = new CustomMenuEntry(playGameRec, "Play Game");
            Rectangle optionRec = new Rectangle(100, 400, 50, 150);
            optionMenuEntry = new CustomMenuEntry(optionRec, "Options");

            // Hook up menu event handlers.
            gameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionMenuEntry.Selected += OptionsMenuEntrySelected;

            IsPopup = true;
            menuEntries.Add(gameMenuEntry);
            menuEntries.Add(optionMenuEntry);
        }


        #endregion

        #region Handle Input

        protected override Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            if (entry is CustomMenuEntry)
            {
                return ((CustomMenuEntry)entry).Bounds;
            }
            return new Rectangle();
        }
        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }
        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }
        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            //Vector2 position = new Vector2(0f, 175f);
            Vector2 playGamePosition = new Vector2(gameMenuEntry.Bounds.X, gameMenuEntry.Bounds.Y);
            playGamePosition.X += gameMenuEntry.Bounds.Width / 2;
            playGamePosition.Y += gameMenuEntry.Bounds.Height / 2;
            Vector2 optionsPosition = new Vector2(optionMenuEntry.Bounds.X, optionMenuEntry.Bounds.Y);
            optionsPosition.X += 
            if (ScreenState == ScreenState.TransitionOn)
                playGamePosition.Y -= transitionOffset * 256;

            else
                playGamePosition.Y += transitionOffset * 512;

            // set the entry's position
            gameMenuEntry.Position = playGamePosition;

            if (ScreenState == ScreenState.TransitionOn)
                optionsPosition.Y += transitionOffset * 256;
            else
                optionsPosition.Y -= transitionOffset * 512;
            optionMenuEntry.Position = optionsPosition;

            //// update each menu entry's location in turn
            //for (int i = 0; i < menuEntries.Count; i++)
            //{
            //    MenuEntry menuEntry = menuEntries[i];

            //    // each entry is to be centered horizontally
            //    position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

            //    if (ScreenState == ScreenState.TransitionOn)
            //        position.X -= transitionOffset * 256;
            //    else
            //        position.X += transitionOffset * 512;

            //    // set the entry's position
            //    menuEntry.Position = position;

            //    // move down for the next entry the size of this entry plus our padding
            //    position.Y += menuEntry.GetHeight(this) + (menuEntryPadding * 2);
            //}
        }
        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.

            bool isSelected = IsActive;

            gameMenuEntry.Draw(this, isSelected, gameTime);
            optionMenuEntry.Draw(this, isSelected, gameTime);
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
        #endregion
    }
}
