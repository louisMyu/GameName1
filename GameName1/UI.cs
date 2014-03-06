using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class UI
    {
        private Texture2D m_StatusBackground;
        public static SpriteFont m_SpriteFont;
        private Texture2D m_FireButton;
        private Color m_FireButtonColor = Color.White;
        private Vector2 FireButtonPosition;
        private Vector2 WeaponSlot1Position;
        private Vector2 WeaponSlot2Position;
        private Color m_Weapon1ButtonColor = Color.White;
        private Color m_Weapon2ButtonColor = Color.White;

        private int CurrentFPS;
        public static int OFFSET = 175;
        private Vector2 m_StatusBackgroundPosition;
        private Vector2 m_StatusBackGroundScale;
        private Texture2D m_Background;

        public int PlayfieldBottom;
        private float GameWidth;
        private float GameHeight;

        private Rectangle m_FireButtonRec;
        private Vector2 m_FireButtonScale;
        public UI()
        {
        }

        public void LoadContent(ContentManager content, float width, float height)
        {
            m_StatusBackground = content.Load<Texture2D>("Line");
            m_SpriteFont = content.Load<SpriteFont>("Retrofont");
            m_FireButton = content.Load<Texture2D>("FireBtn");
            m_StatusBackgroundPosition = new Vector2(0, 0);
            m_StatusBackGroundScale = Utilities.GetSpriteScaling(new Vector2(OFFSET, height), new Vector2(m_StatusBackground.Width, m_StatusBackground.Height));
            PlayfieldBottom = OFFSET;
            GameWidth = width;
            GameHeight = height;
            m_Background = content.Load<Texture2D>("Louis-game-background");
            FireButtonPosition = new Vector2((PlayfieldBottom/2)  - (m_FireButton.Width/2), GameHeight - m_FireButton.Height - 150);
            WeaponSlot1Position = new Vector2(FireButtonPosition.X + (m_FireButton.Width / 2), FireButtonPosition.Y - 900 + (m_FireButton.Height / 2));
            WeaponSlot2Position = new Vector2(FireButtonPosition.X + (m_FireButton.Width / 2), WeaponSlot1Position.Y + m_FireButton.Height + 150);
            m_FireButtonRec = new Rectangle((int)FireButtonPosition.X, (int)FireButtonPosition.Y, m_FireButton.Width+20, 300);
            m_FireButtonScale = Utilities.GetSpriteScaling(new Vector2(m_FireButtonRec.Width, m_FireButtonRec.Height), new Vector2(m_FireButton.Width, m_FireButton.Height));
        }

        public void Update(Player p, int fps)
        {
            CurrentFPS = fps;
        }

        public void ProcessInput(List<Vector2> vecList, Player p, List<GameObject> AllItems)
        {
            m_FireButtonColor = Color.White;
            Vector2 playerVec = new Vector2(-1, -1);
            bool stopPlayer = false;
            p.isFireButtonDown = false;
            foreach (Vector2 vec in vecList) {
                //give a little leeway so its smoother to touch the bottom of the playfield
                //the player movement clamping will prevent it going off screen
                if (vec.X < PlayfieldBottom - 20)
                {
                    //in the fire button area
                    if (Utilities.PointIntersectsRectangle(vec, m_FireButtonRec))
                    {
                        m_FireButtonColor = Color.Orange;
                        stopPlayer = false;
                        p.isFireButtonDown = true;
                    }
                    Rectangle temp = new Rectangle((int)WeaponSlot1Position.X - (m_FireButton.Width / 2), (int)WeaponSlot1Position.Y - (m_FireButton.Height / 2), m_FireButton.Width, m_FireButton.Height);
                    if (Utilities.PointIntersectsRectangle(vec, temp))
                    {
                        if (p.WeaponSlot1Magic != null)
                        {
                            p.WeaponSlot1Magic.GetEffect(AllItems);
                        }
                        p.WeaponSlot1Magic = null;
                    }
                    temp = new Rectangle((int)WeaponSlot2Position.X - (m_FireButton.Width / 2), (int)WeaponSlot2Position.Y - (m_FireButton.Height / 2), m_FireButton.Width, m_FireButton.Height);
                    if (Utilities.PointIntersectsRectangle(vec, temp))
                    {
                        if (p.WeaponSlot1Magic != null)
                        {
                            p.WeaponSlot1Magic.GetEffect(AllItems);
                        }
                    }
                }
                else
                {
                    playerVec.X = vec.X;
                    playerVec.Y = vec.Y;
                    p.ProcessInput(playerVec, true, stopPlayer);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Player p)
        {
            spriteBatch.Draw(m_StatusBackground, m_StatusBackgroundPosition, null, Color.White, 0.0f, new Vector2(0.0f,0.0f), m_StatusBackGroundScale, SpriteEffects.None, 0.0f);
            //TODO CHANGE THE MAGIC NUMBERS HERE                                                          \/\/\/
            spriteBatch.DrawString(m_SpriteFont, "Life: " + p.LifeTotal, new Vector2(PlayfieldBottom - 50, GameHeight - 550), Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(m_SpriteFont, "XP: " + p.Score, new Vector2(PlayfieldBottom - 80, GameHeight - 550), Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(m_FireButton, FireButtonPosition, null, m_FireButtonColor, 0.0f, new Vector2(0, 0), m_FireButtonScale, SpriteEffects.None, 0);

            Texture2D tempTex = null;
            if (p.WeaponSlot1Magic != null)
            {
                tempTex = p.WeaponSlot1Magic.GetTexture();
                float w_scale = m_FireButton.Width / tempTex.Width;
                float h_scale = m_FireButton.Height / tempTex.Height;
                spriteBatch.Draw(tempTex, WeaponSlot1Position, null, m_Weapon1ButtonColor, Utilities.DegreesToRadians(90.0f), new Vector2(tempTex.Width / 2, tempTex.Height / 2), new Vector2(w_scale, h_scale), SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(m_FireButton, WeaponSlot1Position, null, Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(m_FireButton.Width / 2, m_FireButton.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0);
            }
            if (p.WeaponSlot2Magic != null)
            {
                spriteBatch.Draw(p.WeaponSlot2Magic.GetTexture(), WeaponSlot2Position, m_Weapon2ButtonColor);
            }
            else
            {
                spriteBatch.Draw(m_FireButton, WeaponSlot2Position, null, Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(m_FireButton.Width / 2, m_FireButton.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0);
            }
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_Background, new Vector2(OFFSET, 0), Color.White);
        }
    }
}
