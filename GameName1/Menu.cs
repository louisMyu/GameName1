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
    public enum GameState
    {
        Menu,
        Playing,
        Upgrade
    }

    public enum MenuState
    {
        Main,
        Weapon,
        PowerUp
    }
    class Menu
    {
        private static Texture2D MenuTexture;
        private static Texture2D tempTex;
        public MenuState State = MenuState.Main;
        private Vector2 BackgroundPosition = new Vector2(30, 30);
        private Vector2 UpgradeWeaponPosition;
        private Vector2 UpgradePowerUpPosition;

        private Rectangle UpgradeWeaponRec;
        private Rectangle UpgradePowerUpRec;
        private Rectangle QuitRec;
        private Vector2 QuitPosition;
        public void LoadContent(ContentManager content)
        {
            MenuTexture = content.Load<Texture2D>("Menu");
            tempTex = content.Load<Texture2D>("WhiteTile");
            UpgradeWeaponPosition.X = BackgroundPosition.X + MenuTexture.Width - 30;
            UpgradeWeaponPosition.Y = BackgroundPosition.Y + 350;
            UpgradeWeaponRec = new Rectangle((int)UpgradeWeaponPosition.X-50, (int)UpgradeWeaponPosition.Y, 50, 250);

            UpgradePowerUpPosition.X = UpgradeWeaponPosition.X - 155;
            UpgradePowerUpPosition.Y = UpgradeWeaponPosition.Y;
            UpgradePowerUpRec = new Rectangle((int)UpgradePowerUpPosition.X - 50, (int)UpgradePowerUpPosition.Y, 50, 250);

            QuitPosition.X = BackgroundPosition.X + 50;
            QuitPosition.Y = BackgroundPosition.Y + 350;
            QuitRec = new Rectangle((int)QuitPosition.X-50, (int)QuitPosition.Y, 50, 250);
        }

        //returns true if quitting
        public GameState Update(List<Vector2> touches, out bool quit)
        {
            quit = false;
            foreach (Vector2 loc in touches)
            {
                switch (State)
                {
                    case MenuState.Main:
                        if (Utilities.PointIntersectsRectangle(loc, UpgradeWeaponRec))
                        {
                            State = MenuState.Weapon;
                        }
                        if (Utilities.PointIntersectsRectangle(loc, QuitRec))
                        {
                            quit = true;
                        }
                        if (Utilities.PointIntersectsRectangle(loc, UpgradePowerUpRec))
                        {
                            State = MenuState.PowerUp;
                        }
                        break;
                    case MenuState.Weapon:
                        return GameState.Playing;
                    case MenuState.PowerUp:
                        Rectangle temp = new Rectangle(200,0, 500, 1280);
                        if (Utilities.PointIntersectsRectangle(loc, temp))
                        {
                            return GameState.Playing;
                        }
                        break;
                }
            }
            return GameState.Menu;
        }
        public void Draw(SpriteBatch spriteBatch, Player p)
        {
            spriteBatch.Draw(MenuTexture, BackgroundPosition, Color.White);
            switch (State)
            {
                case MenuState.Main:
                    Color temp = new Color(0, 255, 255, 0);
                    Vector2 scale = Utilities.GetSpriteScaling(new Vector2(UpgradeWeaponRec.Width, UpgradeWeaponRec.Height), new Vector2(tempTex.Width, tempTex.Height));
                    spriteBatch.Draw(tempTex, new Vector2(UpgradeWeaponRec.X, UpgradeWeaponRec.Y), null, temp, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
                    scale = Utilities.GetSpriteScaling(new Vector2(QuitRec.Width, QuitRec.Height), new Vector2(tempTex.Width, tempTex.Height));
                    spriteBatch.Draw(tempTex, new Vector2(QuitRec.X, QuitRec.Y), null, temp, 0, new Vector2(0,0), scale, SpriteEffects.None, 0.0f);
                    scale = Utilities.GetSpriteScaling(new Vector2(UpgradePowerUpRec.Width, UpgradePowerUpRec.Height), new Vector2(tempTex.Width, tempTex.Height));
                    spriteBatch.Draw(tempTex, new Vector2(UpgradePowerUpRec.X, UpgradePowerUpRec.Y), null, temp, 0, new Vector2(0,0), scale, SpriteEffects.None, 0.0f);

                    spriteBatch.DrawString(UI.m_SpriteFont, "Weapons", UpgradeWeaponPosition, Color.Black, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
                    spriteBatch.DrawString(UI.m_SpriteFont, "Power Ups", UpgradePowerUpPosition, Color.Black, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
                    spriteBatch.DrawString(UI.m_SpriteFont, "Quit", QuitPosition, Color.Black, Utilities.DegreesToRadians(90.0f), new Vector2(0, 0), 1f, SpriteEffects.None, 0.0f);
                    break;
                case MenuState.Weapon:
                    break;
                case MenuState.PowerUp:

                    break;
            }
        }
        private class WeaponMenu
        {

        }
        private class PowerUpMenu
        {

        }
    }
}
