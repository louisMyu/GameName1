﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameName1
{
    class CustomMenuEntry : MenuEntry
    {
        private Rectangle Bounds;

        public CustomMenuEntry(string text)
            : base(text)
        {
        }
        public CustomMenuEntry(Rectangle bounds, string text)
            : base(text)
        {
            Bounds = bounds;
        }
        public override int GetHeight()
        {
            return Bounds.Width;
        }
        public override int GetWidth()
        {
            return Bounds.Height;
        }
        public override void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 textSize = font.MeasureString(text);
            Vector2 origin = textSize / 2;

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, new Vector2(1,1), SpriteEffects.None, 0);
        }
        public override void UpdatePosition()
        {
            //
        }
    }
}
