using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class UpgradeMenuScreen : GameScreen
    {
        #region Fields

        Rectangle m_FinalMainScreenRec;
        Rectangle m_CurrentMainScreenRec;
        private enum SelectedMenu
        {
            Weapon,
            Cheat,
            Store
        }

        //static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        //static int currentLanguage = 0;

        //static bool frobnicate = true;

        //static int elf = 23;

        private Viewport Viewport;
        //width of the screen that is the category selector
        private int CategoryWidth;
        //height of a single category
        private int CategoryHeight;
        private const int NUMBER_OF_CATEGORIES = 3;

        private Rectangle UpgradeWeaponRec;
        private MenuCategory UpgradeWeaponCategory;
        private Rectangle UpgradeCheatsRec;
        private MenuCategory UpgradeCheatsCategory;
        private Rectangle StoreRec;
        private MenuCategory StoreCategory;
        private MenuCategory m_SelectedMenu;

        private List<MenuCategory> CategoryList = new List<MenuCategory>();
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public UpgradeMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Viewport = ScreenManager.GraphicsDevice.Viewport;
            CategoryWidth = Viewport.Height / 5;
            CategoryHeight = Viewport.Width / NUMBER_OF_CATEGORIES;
            m_FinalMainScreenRec = new Rectangle(0, CategoryWidth, Viewport.Width, Viewport.Height - CategoryWidth);
            StoreRec = new Rectangle(0, 0, CategoryHeight, CategoryWidth);
            StoreCategory = new MenuCategory(StoreRec, "Store", Color.Blue, m_FinalMainScreenRec);
            StoreCategory.HandleTap += StoreMenuTapped;
            StoreCategory.HandleDrag += StoreMenuDragged;
            CategoryList.Add(StoreCategory);
            UpgradeCheatsRec = new Rectangle(StoreRec.X + StoreRec.Width, 0, CategoryHeight, CategoryWidth);
            UpgradeCheatsCategory = new MenuCategory(UpgradeCheatsRec, "Upgrade Cheats", Color.Red, m_FinalMainScreenRec);
            UpgradeCheatsCategory.HandleTap += CheatMenuTapped;
            UpgradeCheatsCategory.HandleDrag += CheatMenuDragged;
            CategoryList.Add(UpgradeCheatsCategory);
            UpgradeWeaponRec = new Rectangle(UpgradeCheatsRec.X + UpgradeCheatsRec.Width, 0, CategoryHeight, CategoryWidth);
            UpgradeWeaponCategory = new MenuCategory(UpgradeWeaponRec, "Upgrade Weapons", Color.Beige, m_FinalMainScreenRec);
            UpgradeWeaponCategory.HandleTap += UpgradeMenuTapped;
            UpgradeWeaponCategory.HandleDrag += UpgradeMenuDragged;
            CategoryList.Add(UpgradeWeaponCategory);

            
            EnabledGestures = GestureType.Tap | GestureType.VerticalDrag;
            foreach (MenuCategory category in CategoryList)
            {
                category.LoadContent();
            }
            m_SelectedMenu = UpgradeWeaponCategory;
            IsPopup = true;
        }

        #endregion

        #region Handle Input
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(Input input)
        {
            if (ScreenState != ScreenState.Active)
            {
                return;
            }
            // we cancel the current menu screen if the user presses the back button
            if (input.IsNewKeyPress(Buttons.Back))
            {
                ExitScreen();
            }

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // iterate the entries to see if any were tapped
                    foreach (MenuCategory category in CategoryList)
                    {
                        if (category.SelectableArea.Contains(tapLocation))
                        {
                            m_SelectedMenu = category;
                            return;
                        }
                    }
                    if (m_FinalMainScreenRec.Contains(tapLocation))
                    {
                        m_SelectedMenu.HandleCategoryTap(tapLocation);
                    }
                }
                else if (gesture.GestureType == GestureType.VerticalDrag)
                {
                    m_SelectedMenu.HandleCategoryDrag(gesture);
                }
            }
        }
        private void UpgradeMenuTapped(object sender, PointEventArgs e)
        {
            Point point = e.Tap;
        }
        private void CheatMenuTapped(object sender, PointEventArgs e)
        {
            Point point = e.Tap;
        }
        private void StoreMenuTapped(object sender, PointEventArgs e)
        {
            Point point = e.Tap;
        }
        private void UpgradeMenuDragged(object sender, GestureEventArgs e)
        {
            GestureSample gesture = e.Gesture;
        }
        private void CheatMenuDragged(object sender, GestureEventArgs e)
        {
            GestureSample gesture = e.Gesture;
        }
        private void StoreMenuDragged(object sender, GestureEventArgs e)
        {
            GestureSample gesture = e.Gesture;
        }
        #endregion
        #region Update and Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            foreach (MenuCategory category in CategoryList)
            {
                category.Update(gameTime);
            }
        }
        private void UpdateMenuLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // update each menu entry's location in turn
            for (int i = 0; i < CategoryList.Count; i++)
            {
                MenuCategory category = CategoryList[i];
                Vector2 position = new Vector2(0f, category.SelectableArea.Y);
                // each entry is to be centered horizontally
                position.X = category.SelectableArea.X;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                category.Position = position;
                category.CurrentPosition.X = (int)position.X;
                category.CurrentPosition.Y = (int)position.Y;
                //// move down for the next entry the size of this entry plus our padding
                //position.Y += category.GetHeight(this) + (menuEntryPadding * 2);
            }
            //do a fade for the actual selection area of the categories
            m_CurrentMainScreenRec = m_FinalMainScreenRec;
            if (ScreenState == ScreenState.TransitionOn)
            {
                m_CurrentMainScreenRec.Y += (int)(transitionOffset * 720);
            }
            else
            {
                m_CurrentMainScreenRec.Y += (int)(transitionOffset * 960);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < CategoryList.Count; i++)
            {
                MenuCategory category = CategoryList[i];

                bool isSelected = IsActive;

                category.Draw(this, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            //Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;

            titlePosition.Y -= transitionOffset * 100;

            //spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
            //                       titleOrigin, titleScale, SpriteEffects.None, 0);
            m_SelectedMenu.DrawSelection(this, gameTime, m_CurrentMainScreenRec);
            spriteBatch.End();
        }
        #endregion
    }
    //represents a rectangle that can be touched
    class MenuCategory : MenuEntry
    {
        Color m_color;
        Rectangle m_selectableArea;
        public Rectangle SelectableArea { get { return m_selectableArea; } }
        public Rectangle CurrentPosition;
        Texture2D texture;
        Texture2D m_SelectedTexture;
        public event EventHandler<GestureEventArgs> HandleDrag;
        public event EventHandler<PointEventArgs> HandleTap;
        //rectangle representing the area to display this menu's selection of things
        Rectangle m_SelectionRec;
        public MenuCategory(Rectangle area, string text, Color _color, Rectangle selectionArea) : base(text)
        {
            m_selectableArea = area;
            m_color = _color;
            m_SelectionRec = selectionArea;
            CurrentPosition = new Rectangle(0, 0, area.Width, area.Height);
        }
        public void LoadContent()
        {
            texture = ScreenManager.Game.Content.Load<Texture2D>("GSMbackground");
            m_SelectedTexture = ScreenManager.Game.Content.Load<Texture2D>("GSMbackground");
        }

        public void Update(GameTime gameTime)
        {
        }
        public void HandleCategoryTap(Point point)
        {
            HandleTap(this, new PointEventArgs(point));
        }
        public void HandleCategoryDrag(GestureSample gesture)
        {
            HandleDrag(this, new GestureEventArgs(gesture));
        }
        public void DrawSelection(UpgradeMenuScreen screen, GameTime gameTime, Rectangle where)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            spriteBatch.Draw(texture, where, m_color);
        }
        public void Draw(UpgradeMenuScreen screen, GameTime gameTime)
        {
            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            spriteBatch.Draw(texture, CurrentPosition, m_color);
            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
        }
    }
    class GestureEventArgs : EventArgs
    {
        GestureSample m_GestureSample;
        public GestureEventArgs(GestureSample gesture)
        {
            m_GestureSample = gesture;
        }
        public GestureSample Gesture { get { return m_GestureSample; } }
    }
    class PointEventArgs : EventArgs
    {
        Point m_Point;
        public PointEventArgs(Point point)
        {
            m_Point = point;
        }
        public Point Tap { get { return m_Point; } }
    }
}
