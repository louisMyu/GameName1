using FarseerPhysics.Dynamics;
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
        public static List<GameObject> AllGameObjects = new List<GameObject>();
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
        }
        public void Update(TimeSpan elapsedTime)
        {
            foreach (GameObject ob in AllGameObjects)
            {
                if (ob is IEnemy)
                {
                    IEnemy temp = ob as IEnemy;
                    if (temp.GetHealth() <= 0)
                    {
                        RemoveObject(ob);
                    }
                }
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

            if (obj is Zombie)
            {
                ((Zombie)obj).CleanBody();
                --NumZombies;
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
            //temp.X = x;
            //temp.Y = y;
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
