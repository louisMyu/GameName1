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
        private static Dictionary<UpgradeFieldEnum, UpgradeField> Upgrade_List = new Dictionary<UpgradeFieldEnum, UpgradeField>();
        private static string SavedSelectedMenu = "Weapons";
        public static void LoadUpgradeFields()
        {
            Upgrade_List.Add(UpgradeFieldEnum.ShotgunDamage, new UpgradeField("Shotgun", 10, 100));
            Upgrade_List.Add(UpgradeFieldEnum.RifleDamage, new UpgradeField("Rifle", 10, 100));
            Upgrade_List.Add(UpgradeFieldEnum.PlasmaDamage, new UpgradeField("Plasma", 10, 100));
        }
        public static UpgradeField GetFieldValue(UpgradeFieldEnum field)
        {
            return Upgrade_List[field];
        }
        public static void SetFieldValue(UpgradeFieldEnum field, UpgradeField val)
        {
            Upgrade_List[field] = val;
        }
        #region Fields

        Rectangle m_FinalMainScreenRec;
        Rectangle m_CurrentMainScreenRec;

        public enum UpgradeFieldEnum
        {
            ShotgunDamage,
            RifleDamage,
            PlasmaDamage
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

        private Rectangle MenuSelectionTotalArea;

        RenderTarget2D SelectionScreenTexture;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public UpgradeMenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Viewport = ScreenManager.GraphicsDevice.Viewport;
            CategoryWidth = Viewport.Height / 5;
            CategoryHeight = Viewport.Width / NUMBER_OF_CATEGORIES;
            m_FinalMainScreenRec = new Rectangle(0, CategoryWidth, Viewport.Width, Viewport.Height - CategoryWidth);
            MenuSelectionTotalArea = new Rectangle(0, 0, Viewport.Width, Viewport.Height - CategoryWidth);
            m_CurrentMainScreenRec = m_FinalMainScreenRec;
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            if (ScreenState == ScreenState.TransitionOn)
            {
                m_CurrentMainScreenRec.Y += (int)(transitionOffset * 720);
            }
            else
            {
                m_CurrentMainScreenRec.Y += (int)(transitionOffset * 960);
            }

            EnabledGestures = GestureType.Tap | GestureType.VerticalDrag | GestureType.Flick;

            SelectionScreenTexture = new RenderTarget2D(ScreenManager.GraphicsDevice, m_FinalMainScreenRec.Width, m_FinalMainScreenRec.Height);
            IsPopup = true;
        }
        public override void LoadContent()
        {
            StoreRec = new Rectangle(0, 0, CategoryHeight, CategoryWidth);
            StoreCategory = CreateStoreMenu();
            StoreCategory.HandleTap += StoreMenuTapped;
            StoreCategory.HandleDrag += StoreMenuDragged;
            CategoryList.Add(StoreCategory);
            UpgradeCheatsRec = new Rectangle(StoreRec.X + StoreRec.Width, 0, CategoryHeight, CategoryWidth);
            UpgradeCheatsCategory = CreateCheatUpgradeMenu();
            UpgradeCheatsCategory.HandleTap += UpgradeMenuTapped;
            UpgradeCheatsCategory.HandleDrag += ScrollMenuDragged;
            CategoryList.Add(UpgradeCheatsCategory);
            UpgradeWeaponRec = new Rectangle(UpgradeCheatsRec.X + UpgradeCheatsRec.Width, 0, CategoryHeight, CategoryWidth);
            UpgradeWeaponCategory = CreateWeaponUpgradeMenu();
            UpgradeWeaponCategory.HandleTap += UpgradeMenuTapped;
            UpgradeWeaponCategory.HandleDrag += ScrollMenuDragged;
            CategoryList.Add(UpgradeWeaponCategory);
            foreach (MenuCategory category in CategoryList)
            {
                category.LoadContent();
            }
            switch (SavedSelectedMenu)
            {
                case "Weapons":
                    m_SelectedMenu = UpgradeWeaponCategory;
                    break;
                case "Cheats":
                    m_SelectedMenu = UpgradeCheatsCategory;
                    break;
                case "Store":
                    m_SelectedMenu = StoreCategory;
                    break;
            }
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
                SavedSelectedMenu = m_SelectedMenu.Type;
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
                else if (gesture.GestureType == GestureType.Flick)
                {

                }
            }
        }
        private void UpgradeMenuTapped(object sender, PointEventArgs e)
        {
            Point point = e.Tap;
            MenuCategory category = (MenuCategory)sender;

            Point pointInSelectionArea = new Point(e.Tap.X - m_CurrentMainScreenRec.X, e.Tap.Y - m_CurrentMainScreenRec.Y);
            category.HandleSelectionAreaTapped(pointInSelectionArea);
        }
        
        private void StoreMenuTapped(object sender, PointEventArgs e)
        {
            Point point = e.Tap;
        }
        private void ScrollMenuDragged(object sender, GestureEventArgs e)
        {
            GestureSample gesture = e.Gesture;
            MenuCategory category = (MenuCategory)sender;
            category.UpdateSlotPosition((int)gesture.Delta.X, (int)gesture.Delta.Y);
        }
        private void ScrollMenuFlicked(object sender, GestureEventArgs e)
        {
            GestureSample gesture = e.Gesture;
            MenuCategory category = (MenuCategory)sender;
            
            //it wont be as simple as just passing a single value, the flick event
            //occurs only once, but the position needs to be updated multiple times even after the flick happens
            //category.UpdateSlotPosition((int)gesture.Delta.X, (int)gesture.Delta.Y);
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
            // make sure our entries are in the right place before we draw them
            UpdateMenuLocations();
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            graphics.SetRenderTarget(SelectionScreenTexture);
            spriteBatch.Begin();
            m_SelectedMenu.DrawSelection(this, gameTime);
            spriteBatch.End();
        }
        private void UpdateMenuLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            int deltaX, deltaY;
            deltaX = 0;

            //do a fade for the actual selection area of the categories
            m_CurrentMainScreenRec = m_FinalMainScreenRec;
            if (ScreenState == ScreenState.TransitionOn)
            {
                deltaY = (int)(transitionOffset * 720);
            }
            else
            {
                deltaY = (int)(transitionOffset * 960);
            }
            m_CurrentMainScreenRec.Y += deltaY;
            // update each menu entry's location in turn
            for (int i = 0; i < CategoryList.Count; i++)
            {
                MenuCategory category = CategoryList[i];
                Vector2 position = new Vector2(0f, category.SelectableArea.Y);
                // each entry is to be centered horizontally
                position.X = category.SelectableArea.X;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 356;
                else
                    position.X += transitionOffset * 712;

                // set the entry's position
                category.Position = position;
                category.CurrentPosition.X = (int)position.X;
                category.CurrentPosition.Y = (int)position.Y;
                //// move down for the next entry the size of this entry plus our padding
                //position.Y += category.GetHeight(this) + (menuEntryPadding * 2);
            }
            
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            
            spriteBatch.Begin();
            spriteBatch.Draw(SelectionScreenTexture, m_CurrentMainScreenRec, Color.White);
            // Draw each menu entry in turn.
            for (int i = 0; i < CategoryList.Count; i++)
            {
                MenuCategory category = CategoryList[i];

                bool isSelected = IsActive;

                category.Draw(this, gameTime);
            }

            //// Make the menu slide into place during transitions, using a
            //// power curve to make things look more interesting (this makes
            //// the movement slow down as it nears the end).
            //float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //// Draw the menu title centered on the screen
            //Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            ////Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = new Color(192, 192, 192) * TransitionAlpha;

            //titlePosition.Y -= transitionOffset * 100;

            //spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
            //                       titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        #endregion

        private int SelectionUpgradeWidth;
        private MenuCategory CreateWeaponUpgradeMenu()
        {
            int NumberOfWeapons = 4;
            SelectionUpgradeWidth = MenuSelectionTotalArea.Height / NumberOfWeapons;
            MenuCategory menu = new MenuCategory("Weapons", UpgradeWeaponRec, "Upgrade Weapons", Color.Beige, MenuSelectionTotalArea);
            List<UpgradeSlot> upgradeSlots = new List<UpgradeSlot>();
            //base widget tree
            for (int i = 0; i < NumberOfWeapons; ++i)
            {
                Rectangle temp = new Rectangle(0, 0, MenuSelectionTotalArea.Width, SelectionUpgradeWidth);
                Rectangle currentSlotPosition = new Rectangle(0, (SelectionUpgradeWidth * i), temp.Width, temp.Height);
                UpgradeSlot slot = new UpgradeSlot(currentSlotPosition, ScreenManager, ScreenManager.Font);
                switch (i)
                {
                    case 0:
                        slot.SetUpgradeField(UpgradeFieldEnum.ShotgunDamage, Upgrade_List[UpgradeFieldEnum.ShotgunDamage]);
                        break;
                    case 1:
                        slot.SetUpgradeField(UpgradeFieldEnum.RifleDamage, Upgrade_List[UpgradeFieldEnum.RifleDamage]);
                        break;
                    case 2:
                        slot.SetUpgradeField(UpgradeFieldEnum.PlasmaDamage, Upgrade_List[UpgradeFieldEnum.PlasmaDamage]);
                        break;
                    default:
                        slot.SetUpgradeField(UpgradeFieldEnum.RifleDamage, Upgrade_List[UpgradeFieldEnum.RifleDamage]);
                        break;
                }
                Color tempColor = new Color(250 - (75*i),75*i,50-(i*10));
                WidgetTree tree = new WidgetTree(temp);
                Rectangle baseSlotDrawArea = new Rectangle(Viewport.Width/2, SelectionUpgradeWidth/2, SelectionUpgradeWidth, MenuSelectionTotalArea.Width);
                tree.AddDrawArea(baseSlotDrawArea, new ColorTexture(TextureBank.GetTexture("GSMbackground"), tempColor));
                WidgetTree slotTop = new WidgetTree(new Rectangle(0,0, baseSlotDrawArea.Width, baseSlotDrawArea.Height));
                Rectangle tapButton = new Rectangle(baseSlotDrawArea.Height / 2, baseSlotDrawArea.Width / 2, 150, 150);
                Rectangle valueArea = new Rectangle(tapButton.X - 200, tapButton.Y, 200, 100);
                Rectangle descriptionArea = new Rectangle(tapButton.X + 200, valueArea.Y, 200, 10);
                slotTop.AddDrawArea(tapButton, new ColorTexture(TextureBank.GetTexture("GSMbackground"), Color.Black));
                slotTop.AddDrawArea(valueArea, slot.ValueString);
                slotTop.AddDrawArea(descriptionArea, slot.Description);
                slotTop.AddHitArea(tapButton);
                tree.AddWidgetTree(slotTop);

                
                slot.SetWidgetTree(tree);
                slot.Value = i * 100;
                upgradeSlots.Add(slot);
            }
            menu.SetUpgradeSlot(upgradeSlots);
            return menu;
        }
        private MenuCategory CreateCheatUpgradeMenu()
        {
            int NumberOfCheats = 6;
            SelectionUpgradeWidth = MenuSelectionTotalArea.Height / NumberOfCheats;
            Rectangle temp = new Rectangle(0, 0, MenuSelectionTotalArea.Width, SelectionUpgradeWidth);
            MenuCategory menu = new MenuCategory("Cheats", UpgradeCheatsRec, "Upgrade Cheats", Color.Red, MenuSelectionTotalArea);
            List<UpgradeSlot> upgradeSlots = new List<UpgradeSlot>();
            //base widget tree
            for (int i = 0; i < NumberOfCheats; ++i)
            {
                Color tempColor = new Color(250 - (75*i),75*i,50-(i*10));
                WidgetTree tree = new WidgetTree(temp);
                Rectangle baseSlotDrawArea = new Rectangle(Viewport.Width / 2, SelectionUpgradeWidth / 2, SelectionUpgradeWidth, MenuSelectionTotalArea.Width);
                tree.AddDrawArea(baseSlotDrawArea, new ColorTexture(TextureBank.GetTexture("GSMbackground"), tempColor));
                Rectangle currentSlotPosition = new Rectangle(0, (SelectionUpgradeWidth * i), temp.Width, temp.Height);
                UpgradeSlot slot = null;
                try
                {
                    slot = new UpgradeSlot(currentSlotPosition, ScreenManager, ScreenManager.Font);
                }
                catch (Exception e)
                {

                }
                slot.SetWidgetTree(tree);
                upgradeSlots.Add(slot);
            }
            menu.SetUpgradeSlot(upgradeSlots);
            return menu;
        }
        private MenuCategory CreateStoreMenu()
        {
            MenuCategory menu = new MenuCategory("Store", StoreRec, "Store", Color.Blue, MenuSelectionTotalArea);
            return menu;
        }
    }
    //represents a rectangle that can be touched
    class MenuCategory : MenuEntry
    {
        public string Type { get; set; }
        List<UpgradeSlot> m_UpgradeSlots;
        Color m_color;
        Rectangle m_selectableArea;
        public Rectangle SelectableArea { get { return m_selectableArea; } }
        public Rectangle CurrentPosition;
        private Rectangle CurrentSelectionAreaRec;
        Texture2D texture;
        Texture2D m_SelectedTexture;
        public event EventHandler<GestureEventArgs> HandleDrag;
        public event EventHandler<PointEventArgs> HandleTap;
        //rectangle representing the area to display this menu's selection of things
        Rectangle m_SelectionRec;
        public MenuCategory(string type, Rectangle area, string text, Color _color, Rectangle selectionArea) : base(text)
        {
            Type = type;
            m_selectableArea = area;
            m_color = _color;
            m_SelectionRec = new Rectangle(0,0, area.Width, area.Height);
            CurrentPosition = new Rectangle(0, 0, area.Width, area.Height);
            CurrentSelectionAreaRec = selectionArea;
        }
        public void LoadContent()
        {
            texture = ScreenManager.Game.Content.Load<Texture2D>("GSMbackground");
            m_SelectedTexture = ScreenManager.Game.Content.Load<Texture2D>("GSMbackground");
        }
        public void SetUpgradeSlot(List<UpgradeSlot> slots)
        {
            m_UpgradeSlots = slots;
        }
        public void UpdateSlotPosition(int deltaX, int deltaY)
        {

            if (m_UpgradeSlots != null)
            {
                foreach (UpgradeSlot slot in m_UpgradeSlots)
                {
                    slot.Update(deltaX, deltaY);
                }
            }
        }
        public void HandleSelectionAreaTapped(Point p)
        {
            foreach (UpgradeSlot slot in m_UpgradeSlots)
            {
                if (slot.PointIsInContainer(p))
                {
                    slot.HandleTap(p);
                }
            }
            
        }
        public void HandleCategoryTap(Point point)
        {
            HandleTap(this, new PointEventArgs(point));
        }
        public void HandleCategoryDrag(GestureSample gesture)
        {
            HandleDrag(this, new GestureEventArgs(gesture));
        }
        public void DrawSelection(UpgradeMenuScreen screen, GameTime gameTime)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            spriteBatch.Draw(texture, CurrentSelectionAreaRec, m_color);
            if (m_UpgradeSlots == null)
            {
                return;
            }
            foreach (UpgradeSlot slot in m_UpgradeSlots)
            {
                slot.Draw(spriteBatch);
            }
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

            Vector2 origin = font.MeasureString(text) / 2;
            spriteBatch.Draw(texture, CurrentPosition, m_color);
            spriteBatch.DrawString(font, text, new Vector2(CurrentPosition.X + CurrentPosition.Width/2, CurrentPosition.Y+CurrentPosition.Height/2), color, Utilities.DegreesToRadians(90f),
                                   origin, 1.0f, SpriteEffects.None, 0);
        }
    }
    class UpgradeSlot
    {
        private UpgradeMenuScreen.UpgradeFieldEnum m_UpgradeField;
        private UpgradeField m_UpgradeValue;

        public ColorString ValueString;
        int m_Value;
        public int Value { get { return m_Value; } set { m_Value = value; } }
        WidgetTree Widgets;
        public ColorString Description;
        private Rectangle FinalContainer;
        private SpriteFont m_Font;
        ScreenManager screenManager;
        public UpgradeSlot(Rectangle baseContainer, ScreenManager manager, SpriteFont font = null)
        {
            screenManager = manager;
            FinalContainer = baseContainer;
            m_Font = font;
            ValueString = new ColorString(font, "Test", Color.Black);    
        }
        public void SetUpgradeField(UpgradeMenuScreen.UpgradeFieldEnum field, UpgradeField val)
        {
            Description = new ColorString(m_Font, val.Description, Color.White);
            m_UpgradeField = field;
            m_UpgradeValue = val;
        }
        public void SetWidgetTree(WidgetTree widg)
        {
            Widgets = widg;
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            Widgets.StartDrawWidgets(_spriteBatch, FinalContainer);
        }
        public void Update(int deltaX, int deltaY)
        {
            FinalContainer.X += (deltaX);
            FinalContainer.Y += (deltaY);
            //Widgets.UpdatePositions(delta);
        }
        public bool PointIsInContainer(Point p)
        {
            if (FinalContainer.Contains(p))
            {
                return true;
            }
            return false;
        }
        public void HandleTap(Point p)
        {
            Point offsetPoint = new Point(p.X - FinalContainer.X, p.Y - FinalContainer.Y);
            Rectangle tempRec = Widgets.CheckCollision(offsetPoint);
            if (tempRec.Width > 0)
            {
                //i should pop up a confirmation dialog box here
                MessageBoxScreen box = new MessageBoxScreen("");
                box.Accepted += box_Accepted;
                box.Cancelled += box_Cancelled;
                screenManager.AddScreen(box, null);
            }
        }

        void box_Cancelled(object sender, EventArgs e)
        {
        }

        void box_Accepted(object sender, EventArgs e)
        {
            m_UpgradeValue.Upgrade();
            ValueString.Text = m_UpgradeValue.GetUpgradeCost().ToString();
            UpgradeMenuScreen.SetFieldValue(this.m_UpgradeField, m_UpgradeValue);
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
