﻿using FarseerPhysics.Dynamics;
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
    public class ObjectManager
    {
        private const int GRID_DIVISIONS_X = 50;
        private const int GRID_DIVISIONS_Y = 50;
        public static List<GameObject> AllGameObjects = new List<GameObject>();
        public static List<GameObject>[][] GameObjectGrid;
        public static Random ZombieRandom = new Random(424242);
        public static long FrameCounter = 0;
        public static bool itemMade = false;
        public static bool face = false;
        public static Player m_Player;
        private ContentManager m_Content;
        private World m_World;
        private static int NumZombies = 0;
        private static int MaxZombies = 5;
        private PowerUp m_PowerUp;
        public void Init(Player p, ContentManager content, World world)
        {
            m_Player = p;
            m_Content = content;
            m_World = world;
        }

        public void LoadContent()
        {
            AllGameObjects = Storage.Load<List<GameObject>>("GameObjects", "ObjectList.dat");
            if (AllGameObjects == null)
            {
                AllGameObjects = new List<GameObject>();
            }
            else
            {
                foreach (GameObject g in AllGameObjects)
                {
                    g.Load(m_World);
                }
            }
            GameObjectGrid = new List<GameObject>[(Game1.GameWidth / GRID_DIVISIONS_X)+1][];
            for (int x = 0; x < GameObjectGrid.Length; ++x )
            {
                GameObjectGrid[x] = new List<GameObject>[(Game1.GameHeight / GRID_DIVISIONS_Y)+1];
                for (int y = 0; y < GameObjectGrid[x].Length; ++y)
                {
                    GameObjectGrid[x][y] = new List<GameObject>();
                }
            }
        }

        public static List<GameObject> GetCell(Vector2 position)
        {
            int x = (int)position.X / GRID_DIVISIONS_X;
            int y = (int)position.Y / GRID_DIVISIONS_Y;

            return GameObjectGrid[x][y];
        }

        public static List<List<GameObject>> GetCellsOfRectangle(Rectangle rec)
        {
            //the logic here should work as long as the rectangle passed in is =< half the size of the
            //grid tiles
            List<List<GameObject>> temp = new List<List<GameObject>>();
            List<GameObject> curCell = GetCell(new Vector2(rec.X, rec.Y));
            if (!temp.Contains(curCell))
            {
                temp.Add(curCell);
            }
            curCell = GetCell(new Vector2(rec.X, rec.Y+rec.Height));
            if (!temp.Contains(curCell))
            {
                temp.Add(curCell);
            }
            curCell = GetCell(new Vector2(rec.X+rec.Width, rec.Y));
            if (!temp.Contains(curCell))
            {
                temp.Add(curCell);
            }
            curCell = GetCell(new Vector2(rec.X+rec.Width, rec.Y+rec.Height));
            if (!temp.Contains(curCell))
            {
                temp.Add(curCell);
            }
            return temp;
        }
        public static void ClearGrid()
        {
            for (int x = 0; x < GameObjectGrid.Length; ++x)
            {
                for (int y = 0; y < GameObjectGrid[0].Length; ++y)
                {
                    GameObjectGrid[x][y].Clear();
                }
            }
        }
        public void Update(TimeSpan elapsedTime)
        {
            List<List<GameObject>> cellsToClean = new List<List<GameObject>>();
            foreach (GameObject ob in AllGameObjects)
            {
                if (ob is IEnemy)
                {
                    if (((GameObject)ob).CanDelete)
                    {
                        cellsToClean.Add(ObjectManager.GetCell(ob.Position));
                        continue;
                    }
                    IEnemy temp = ob as IEnemy;
                    if (temp.GetHealth() <= 0)
                    {
                        RemoveObject(ob);
                    }
                    cellsToClean.Add(ObjectManager.GetCell(ob.Position));
                }
            }
            foreach (List<GameObject> cell in cellsToClean)
            {
                cell.RemoveAll(x => x.CanDelete);
            }
            AllGameObjects.RemoveAll(x => x.CanDelete);
            if (FrameCounter % 100 == 0 && NumZombies < MaxZombies)
            {
                SpawnZombie();
            }
            if (FrameCounter > 1000)
            {
                MakeItem();
                FrameCounter = 0;
                MakeSlime();
            }
            else
            {
                ++FrameCounter;
            }
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (GameObject g in AllGameObjects)
            {
                if (g is PowerUp)
                {
                    _spriteBatch.Draw(g.Texture, g.Position, null, Color.White, Utilities.DegreesToRadians(90.0f), new Vector2(g.Texture.Width / 2, g.Texture.Height / 2), new Vector2(1,1), SpriteEffects.None, 0);
                }
                else
                {
                    g.Draw(_spriteBatch);
                }
            }
        }
        public static void RemoveObject(GameObject obj)
        {

            if (obj is IEnemy)
            {
                ((IEnemy)obj).CleanBody();
                if (obj is Zombie)
                {
                    --NumZombies;
                }
            }
            obj.CanDelete = true;
        }

        private void SpawnZombie()
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
                if (distanceFromPlayer.LengthSquared() >= (300.0f * 300f))
                {
                    nearPlayer = false;
                }
            }
            Zombie z = new Zombie();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            z.Position = temp;
            z.LoadContent(m_World);
            AllGameObjects.Add(z);
            ++NumZombies;
        }

        public void ResetGame()
        {
            FrameCounter = 0;
            itemMade = false;
            face = false;
            m_Player.Score = 0;
            NumZombies = 0;
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
                if (distanceFromPlayer.LengthSquared() >= (150.0f * 150.0f))
                {
                    nearPlayer = false;
                }
            }
            int powerUpType = ZombieRandom.Next(2);
            if (powerUpType == 0) 
            {
                //m_PowerUp = new CheatPowerUp((CheatPowerUp.CheatTypes)ZombieRandom.Next(3));
                m_PowerUp = new CheatPowerUp(CheatPowerUp.CheatTypes.Time);
            }
            else if (powerUpType == 1)
            {
                m_PowerUp = new WeaponPowerUp((WeaponPowerUp.WeaponType)ZombieRandom.Next(3));
            }
            Vector2 temp = new Vector2();
            temp.X = MathHelper.Clamp(x, 0 + UI.OFFSET, Game1.GameWidth-15);
            temp.Y = MathHelper.Clamp(y, 0, Game1.GameHeight-15);
            m_PowerUp.Position = temp;
            m_PowerUp.LoadContent();
            ObjectManager.AllGameObjects.Add(m_PowerUp);
        }
        //probably should add spawn face in here
        private void MakeSlime()
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
                if (distanceFromPlayer.LengthSquared() >= (200.0f * 200f))
                {
                    nearPlayer = false;
                }
            }
            Slime z = new Slime();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            z.Position = temp;
            z.LoadContent(m_World);
            AllGameObjects.Add(z);
        }
    }
}
