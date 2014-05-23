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

        Rectangle m_viewPort;

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
        private SelectedMenu m_SelectedMenu;

        private List<MenuCategory> CategoryList = new List<MenuCategory>();
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public UpgradeMenuScreen()
        {
            Viewport = ScreenManager.GraphicsDevice.Viewport;
            CategoryWidth = Viewport.Height / 5;
            CategoryHeight = Viewport.Width / NUMBER_OF_CATEGORIES;
            m_viewPort = new Rectangle(0, CategoryWidth, Viewport.Width, Viewport.Height - CategoryWidth);
            StoreRec = new Rectangle(0, 0, CategoryHeight, CategoryWidth);
            StoreCategory = new MenuCategory(StoreRec, "Store", Color.Blue);
            CategoryList.Add(StoreCategory);
            UpgradeCheatsRec = new Rectangle(0, UpgradeWeaponRec.Height, CategoryWidth, CategoryHeight);
            UpgradeCheatsCategory = new MenuCategory(UpgradeCheatsRec, "Upgrade Cheats", Color.Red);
            CategoryList.Add(UpgradeCheatsCategory);
            UpgradeWeaponRec = new Rectangle(0, 0, CategoryWidth, CategoryHeight);
            UpgradeWeaponCategory = new MenuCategory(UpgradeWeaponRec, "Upgrade Weapons", Color.Beige);
            CategoryList.Add(UpgradeWeaponCategory);

            m_SelectedMenu = SelectedMenu.Weapon;
        }



        #endregion

        #region Handle Input
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(Input input)
        {
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
                        if (category.SelectablelArea.Contains(tapLocation))
                        {
                            category.OnSelectEntry(PlayerIndex.One);
                            return;
                        }
                    }
                }
            }
        }
        private void UpgradeMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            m_SelectedMenu = SelectedMenu.Weapon;
        }
        private void CheatMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            m_SelectedMenu = SelectedMenu.Cheat;
        }
        private void StoreMenuSelected(object sender, PlayerIndexEventArgs e)
        {
            m_SelectedMenu = SelectedMenu.Store;
        }
        #endregion
        #region Update and Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            switch (m_SelectedMenu)
            {
                case SelectedMenu.Weapon:
                    break;
                case SelectedMenu.Cheat:
                    break;
                case SelectedMenu.Store:
                    break;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            
        }
        #endregion
    }
    //represents a rectangle that can be touched
    class MenuCategory : MenuEntry
    {
        Color m_color;
        Rectangle m_selectableArea;
        public Rectangle SelectablelArea { get { return m_selectableArea; } }
        Texture2D texture;
        public MenuCategory(Rectangle area, string text, Color _color) : base(text)
        {
            m_selectableArea = area;
            m_color = _color;
        }
        public void LoadContent()
        {
            texture = ScreenManager.Game.Content.Load<Texture2D>("GSMbackground");
        }

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            base.OnSelectEntry(playerIndex);
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

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            spriteBatch.Draw(texture, m_selectableArea, m_color);
            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
