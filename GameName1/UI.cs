using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class UI
    {
        private TimeSpan TimeToDeath;
        private Texture2D m_StatusBackground;
        public static SpriteFont m_SpriteFont;
        private Texture2D m_FireButton;
        private Texture2D m_ThumbStickBottomTexture;
        private Texture2D m_ThumbStickTopTexture;

        private Color m_FireButtonColor = Color.White;
        private Color m_StopButtonColor = Color.White;
        private Vector2 FireButtonPosition;
        private Vector2 StopButtonPosition;
        private Vector2 WeaponSlotPosition;
        private Color m_Weapon1ButtonColor = Color.White;

        public static int OFFSET = 175;
        private Vector2 m_StatusBackgroundPosition;
        private Vector2 m_StatusBackGroundScale;
        private Texture2D m_Background;

        public int PlayfieldBottom;
        private float GameWidth;
        private float GameHeight;
        
        private Rectangle m_FireButtonRec;
        private Vector2 m_FireButtonScale;
        public static Rectangle StopButtonRec;
        private Vector2 m_StopButtonScale;
        private Rectangle WeaponSlotRec;
        private Vector2 WeaponSlotScale;
        private Vector2 ThumbStickPoint;
        //how much the thumbstick is currently offset from the center in pixels
        public static Vector2 ThumbStickPointOffset;
        private int ThumbStickPointId;
        private bool ThumbStickPressed;
        private SpriteFont ColunaFont;
        public static float ThumbStickAngle;

        private int BackGroundHueCounter = -250;
        public Color BackGroundHueColor = new Color(255,0,0);
        private const int MAX_PERIOD = 600;
        private const int MIN_PERIOD = 40;
        private int m_Period;
        //start increasing oscillation at 30 seconds
        private const int OSCILLATE_START = 10;
        private const int SCALE = 200;

        private List<ExplodedPart> BakedGibs = new List<ExplodedPart>();
        public static List<ExplodedPart> ActiveGibs = new List<ExplodedPart>();

        public UI()
        {
        }

        public void LoadContent(ContentManager content, float width, float height)
        {
            m_StatusBackground = content.Load<Texture2D>("Line");
            m_SpriteFont = content.Load<SpriteFont>("Retrofont");
            ColunaFont = content.Load<SpriteFont>("ColunaFont");
            m_FireButton = content.Load<Texture2D>("FireBtn");
            m_ThumbStickBottomTexture = content.Load<Texture2D>("ThumbstickBottom");
            m_ThumbStickTopTexture = content.Load<Texture2D>("ThumbstickTop");

            m_StatusBackgroundPosition = new Vector2(0, 0);
            m_StatusBackGroundScale = Utilities.GetSpriteScaling(new Vector2(OFFSET, height), new Vector2(m_StatusBackground.Width, m_StatusBackground.Height));
            PlayfieldBottom = OFFSET;
            GameWidth = width;
            GameHeight = height;
            m_Background = content.Load<Texture2D>("Louis-game-background");
            //FireButtonPosition = new Vector2((PlayfieldBottom/2)  - (m_FireButton.Width/2), GameHeight - m_FireButton.Height - 150);
            FireButtonPosition = new Vector2(0, GameHeight - m_FireButton.Height - 150);
            StopButtonPosition = new Vector2(FireButtonPosition.X, 60);
            m_FireButtonRec = new Rectangle((int)FireButtonPosition.X, (int)FireButtonPosition.Y, PlayfieldBottom, 300);
            m_FireButtonScale = Utilities.GetSpriteScaling(new Vector2(m_FireButtonRec.Width, m_FireButtonRec.Height), new Vector2(m_FireButton.Width, m_FireButton.Height));
            StopButtonRec = new Rectangle((int)StopButtonPosition.X, (int)StopButtonPosition.Y, PlayfieldBottom, PlayfieldBottom);
            m_StopButtonScale = Utilities.GetSpriteScaling(new Vector2(StopButtonRec.Width, StopButtonRec.Height), new Vector2(m_ThumbStickBottomTexture.Width, m_ThumbStickBottomTexture.Height));
            //scaling from double playFieldBottom so that it is square
            WeaponSlotScale = Utilities.GetSpriteScaling(new Vector2(PlayfieldBottom, PlayfieldBottom), new Vector2(m_FireButton.Width, m_FireButton.Height));
            WeaponSlotPosition = new Vector2(FireButtonPosition.X + ((m_FireButton.Width*WeaponSlotScale.X)/2), StopButtonPosition.Y + StopButtonRec.Height + 150 + ((m_FireButton.Height*WeaponSlotScale.Y)/2));
            WeaponSlotRec = new Rectangle((int)(WeaponSlotPosition.X - ((m_FireButton.Width*WeaponSlotScale.X)/2)), (int)(WeaponSlotPosition.Y - ((m_FireButton.Height*WeaponSlotScale.Y)/2)), PlayfieldBottom, PlayfieldBottom);
            ThumbStickPressed = false;
            ThumbStickPoint = StopButtonPosition;
            ActiveGibs.Clear();

            Vector2 viewport = new Vector2(GameWidth-PlayfieldBottom, GameHeight);
            Vector2 textSize = ColunaFont.MeasureString("00:00:00");
            DeathTimerScale = Utilities.GetSpriteScaling(new Vector2((int)(GameHeight * 0.5), (int)(GameWidth * 0.5)), textSize);
            Vector2 textPosition = (viewport - textSize) / 2;
            textPosition.X += (textSize.X / 2) + PlayfieldBottom/2;
            textPosition.Y += textSize.Y / 2;
            DeathTimerPosition = textPosition;
            DeathTimerOrigin = new Vector2(textSize.X / 2, textSize.Y / 2);

            textSize = ColunaFont.MeasureString("00");
            CountdownScale = Utilities.GetSpriteScaling(new Vector2((int)(GameHeight * 0.5), (int)(GameWidth * 0.5)), textSize);
            textPosition = (viewport - textSize) / 2;
            textPosition.X += (textSize.X / 2) ;
            textPosition.Y += textSize.Y / 2;
            CountdownPosition = textPosition;
            CountdownOrigin = new Vector2(textSize.X / 2, textSize.Y / 2);
        }

        public void Update(TimeSpan elapsedTime)
        {
            m_Period = MAX_PERIOD;
            //if (timeToDeath.Seconds <= OSCILLATE_START)
            //{
            //    m_Period = (int)(timeToDeath.TotalSeconds / OSCILLATE_START * (MAX_PERIOD - MIN_PERIOD)) + MIN_PERIOD;
            //}
            if (TimeToDeath.TotalSeconds <= OSCILLATE_START)
            {
                m_Period = MIN_PERIOD;
            }
            if (BackGroundHueCounter >= m_Period + 1)
            {
                BackGroundHueCounter = 0;
            }
            int delta = (int)(Math.Sin(BackGroundHueCounter * 2 * Math.PI / m_Period) * (SCALE / 2) + (SCALE / 2));
            ++BackGroundHueCounter;
            BackGroundHueColor = new Color(255 - delta, delta, 0);
            for (int i = 0; i < ActiveGibs.Count; ++i)
            {
                ExplodedPart part = ActiveGibs[i];
                bool hasStopped;
                part.Update(out hasStopped);
                if (hasStopped)
                {
                    BakedGibs.Add(part);
                    ActiveGibs.Remove(part);
                    i--;
                }
            }
        }

        public void ProcessInput(Player p, TouchCollection input)
        {
            m_FireButtonColor = Color.White;
            m_StopButtonColor = Color.White;
            p.Moving = true;
            bool isFireDown = false;
            bool isStopDown = false;
            foreach (TouchLocation touch in input) {
                if (touch.Id == ThumbStickPointId)
                {
                    if (touch.State == TouchLocationState.Released)
                    {
                        ThumbStickPressed = false;
                        ThumbStickPoint = StopButtonPosition;
                        ThumbStickPointOffset = new Vector2(0, 0);
                        continue;
                    }
                    ThumbStickPointOffset = new Vector2(touch.Position.X - (StopButtonPosition.X + (StopButtonRec.Width / 2)), touch.Position.Y - (StopButtonPosition.Y + (StopButtonRec.Height / 2)));
                }
                if (touch.State == TouchLocationState.Released)
                {
                    continue;
                }
                Vector2 vec = touch.Position;
                //give a little leeway so its smoother to touch the bottom of the playfield
                //the player movement clamping will prevent it going off screen
                if (vec.X < PlayfieldBottom - 20)
                {
                    //in the fire button area
                    if (Utilities.PointIntersectsRectangle(vec, m_FireButtonRec))
                    {
                        m_FireButtonColor = Color.Orange;
                        isFireDown = true;
                    }
                    if (Utilities.PointIntersectsRectangle(vec, StopButtonRec))
                    {
                        if (ThumbStickPressed && ThumbStickPointId == touch.Id && touch.State == TouchLocationState.Moved)
                        {
                            m_StopButtonColor = Color.Orange;
                            isStopDown = true;
                            //position to draw the thumbstick, offset for origin placement
                            ThumbStickPoint = new Vector2(vec.X - StopButtonRec.Width / 2, vec.Y - StopButtonRec.Height / 2);
                            ThumbStickAngle = (float)Math.Atan2(vec.Y - (StopButtonPosition.Y +(StopButtonRec.Height / 2)), vec.X - (StopButtonPosition.X + (StopButtonRec.Width / 2)));
                        }
                        else if (!ThumbStickPressed)
                        {
                            m_StopButtonColor = Color.Orange;
                            isStopDown = true;
                            ThumbStickPointId = touch.Id;
                            ThumbStickPressed = true;
                            //position to draw the thumbstick, offset for origin placement
                            ThumbStickPoint = new Vector2(vec.X - StopButtonRec.Width / 2, vec.Y - StopButtonRec.Height / 2);
                            ThumbStickAngle = (float)Math.Atan2(vec.Y - (StopButtonPosition.Y + (StopButtonRec.Height / 2)), vec.X - (StopButtonPosition.X + (StopButtonRec.Width / 2)));
                            ThumbStickPointOffset = new Vector2(vec.X - (StopButtonPosition.X + (StopButtonRec.Width/2)), vec.Y - (StopButtonPosition.Y + (StopButtonRec.Height/2)));
                        }
                    }
                    if (Utilities.PointIntersectsRectangle(vec, WeaponSlotRec))
                    {
                        p.StartCheatEffect();
                    }
                }
            }
            p.ProcessInput(isFireDown, isStopDown);
        }

        public void Draw(SpriteBatch spriteBatch, Player p)
        {
            spriteBatch.Draw(m_StatusBackground, m_StatusBackgroundPosition, null, Color.White, 0.0f, new Vector2(0.0f,0.0f), m_StatusBackGroundScale, SpriteEffects.None, 0.0f);
            //TODO CHANGE THE MAGIC NUMBERS HERE                                                          \/\/\/
            spriteBatch.DrawString(m_SpriteFont, "Life: " + p.LifeTotal, new Vector2(PlayfieldBottom - 50, GameHeight - 550), Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(m_SpriteFont, "XP: " + p.Score, new Vector2(PlayfieldBottom - 80, GameHeight - 550), Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(m_FireButton, FireButtonPosition, null, m_FireButtonColor, 0.0f, new Vector2(0, 0), m_FireButtonScale, SpriteEffects.None, 0);
            spriteBatch.Draw(m_ThumbStickBottomTexture, StopButtonPosition, null, m_StopButtonColor, 0.0f, new Vector2(0,0), m_StopButtonScale, SpriteEffects.None, 0);
            
            spriteBatch.Draw(m_ThumbStickTopTexture, ThumbStickPoint, null, Color.White, 0.0f, new Vector2(0, 0), m_StopButtonScale, SpriteEffects.None, 0);

            if (p.WeaponSlot1Magic != null)
            {
                Texture2D tempTex = p.WeaponSlot1Magic.Texture;
                Vector2 tempScale = Utilities.GetSpriteScaling(new Vector2(WeaponSlotRec.Width, WeaponSlotRec.Height), new Vector2(tempTex.Width, tempTex.Height));
                spriteBatch.Draw(tempTex, WeaponSlotPosition, null, m_Weapon1ButtonColor, Utilities.DegreesToRadians(90.0f), new Vector2(tempTex.Width / 2, tempTex.Height / 2), tempScale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(m_FireButton, WeaponSlotPosition, null, Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(m_FireButton.Width / 2, m_FireButton.Height / 2), WeaponSlotScale, SpriteEffects.None, 0);
            }
            if (p.DrawRedFlash)
            {
                spriteBatch.Draw(p.RedFlashTexture, new Vector2(PlayfieldBottom, 0), null, Color.White, 0, new Vector2(0,0),Utilities.GetSpriteScaling(new Vector2(GameWidth-PlayfieldBottom, GameHeight), new Vector2(p.RedFlashTexture.Width, p.RedFlashTexture.Height)) ,SpriteEffects.None, 0);
            }
        }
        public void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_Background, new Vector2(GameWidth / 2, GameHeight / 2), null, Color.White, 0.0f, new Vector2(GameWidth / 2, GameHeight / 2), 
                            Utilities.GetSpriteScaling(new Vector2(GameWidth, GameHeight), new Vector2(m_Background.Width, m_Background.Height)), SpriteEffects.None, 0.0f);
        }
        public void DrawActiveGibs(SpriteBatch spriteBatch)
        {
            foreach (ExplodedPart part in ActiveGibs)
            {
                part.Draw(spriteBatch, BackGroundHueColor);
            }
        }
        public void DrawBakedGibs(SpriteBatch spriteBatch)
        {
            foreach (ExplodedPart part in BakedGibs)
            {
                part.DrawOffset(spriteBatch, BackGroundHueColor);
                part.CleanBody();
            }
            BakedGibs.Clear();
        }

        private Vector2 DeathTimerScale;
        private Vector2 DeathTimerOrigin;
        private Vector2 DeathTimerPosition;
        public void DrawDeathTimer(SpriteBatch spriteBatch)
        {
            string timeToDeathString = TimeToDeath.ToString(@"mm\:ss\:ff");
            spriteBatch.DrawString(ColunaFont, timeToDeathString, DeathTimerPosition, Color.Blue * 0.45f, Utilities.DegreesToRadians(90.0f), 
                                    DeathTimerOrigin, DeathTimerScale, SpriteEffects.None, 0.0f);
        }

        private Vector2 CountdownScale;
        private Vector2 CountdownOrigin;
        private Vector2 CountdownPosition;
        public void DrawCountdown(SpriteBatch spriteBatch, TimeSpan countdown)
        {
            string countdownString = countdown.ToString(@"ss");
            spriteBatch.DrawString(ColunaFont, countdownString, CountdownPosition, Color.Blue * 0.45f, Utilities.DegreesToRadians(90.0f),
                         CountdownOrigin, CountdownScale, SpriteEffects.None, 0.0f);
        }

        public void SetTimeToDeath(TimeSpan time)
        {
            TimeToDeath = time;
        }

    }
}
