using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Weapon
    {
        protected struct ShotInfo
        {
            public Vector2 Position;
            public float Rotation;
            int NumberOfBullets;
            //left most of a spread shot
            float LeftAngle;
            public int NumFrames;
            public ShotInfo(Vector2 pos, float rot, int numBul, float left, int frames)
            {
                Position = pos;
                Rotation = rot;
                NumberOfBullets = numBul;
                LeftAngle = left;
                NumFrames = frames;
            }
        }

        public bool Firing
        {
            get;
            set;
        }
        //spread of the bullets
        private float m_Spread;
        public float Spread
        {
            get
            {
                if (NumberOfBullets <= 1)
                {
                    return 0;
                }
                return m_Spread;
            }
            set
            {
                m_Spread = value;
            }
        }
        public int NumberOfBullets
        {
            get;
            set;
        }

        //per second
        public float FireRate
        {
            get;
            set;
        }
        protected static Random WEAPON_RANDOM = new Random();
        protected float m_ElapsedTime = 0.0f;
        public Weapon() 
        {
        }

        //this should be called every update if it exists for the player
        public virtual void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, int weaponLength)
        {
            //elapsed time since last update
            m_ElapsedTime += elapsedTime;
        }

        public virtual bool CheckCollision(GameObject ob)
        {
            return false;
        }

        public virtual void DrawWeapon(SpriteBatch _spritebatch, Vector2 position, float rot)
        {
        }

        public virtual void DrawBlast(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
        }

        public virtual bool CanFire()
        {
            if (m_ElapsedTime >= 1.0 / FireRate)
            {
                m_ElapsedTime = 0;
                return true;
            }
            return false;
        }
    }
}
