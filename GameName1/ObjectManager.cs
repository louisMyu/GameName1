using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private Player m_Player;
        private ContentManager m_Content;
        private World m_World;
        private static int NumZombies = 0;
        private static int MaxZombies = 5;
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
                    g.Load(m_Content, m_World);
                }
            }
        }
        public void Update()
        {
            if (FrameCounter % 20 == 0 && NumZombies < MaxZombies)
            {
                SpawnZombie();
            }
            if (FrameCounter > 5000)
            {
                FrameCounter = 0;
            }
            else
            {
                ++FrameCounter;
            }
            //if (ZombieTimer >= ZombieSpawnTimer && NumZombies< MaxZombies)
            //{
            //    SpawnZombie();
            //    ZombieTimer = 0;
            //    if (ZombieSpawnTimer > 10) 
            //    {
            //        ZombieSpawnTimer -= 1;
            //    }
            //    if (ZombieSpawnTimer < 10)
            //    {
            //        ZombieSpawnTimer = 10;
            //    }
            //}
            //if (GameTimer >= 10 && !itemMade)
            //{
            //    MakeItem();
            //    itemMade = true;
            //}
            //if (GameTimer >= 25 && !face)
            //{
            //    SpawnFace();
            //    face = true;
            //}
            //merged from desktop, these comments are necessary
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
                if (distanceFromPlayer.LengthSquared() >= (150.0f * 150f))
                {
                    nearPlayer = false;
                }
            }
            Zombie z = new Zombie();
            Vector2 temp = new Vector2();
            temp.X = x;
            temp.Y = y;
            z.Position = temp;
            z.LoadContent(m_Content, m_World);
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

        public static void RemoveObject(GameObject obj)
        {

            if (obj is Zombie)
            {
                ((Zombie)obj).CleanBody();
                --NumZombies;
            }
            if (AllGameObjects.Contains(obj))
            {
                AllGameObjects.Remove(obj);
            }
        }
        //probably should add spawn face in here
    }
}
