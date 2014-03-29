using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    [DataContract]
    public class Weapon
    {
        [DataMember]
        public float Knockback { get; set; }
        [IgnoreDataMember]
        protected int m_SightRange;
        [DataMember]
        public int SightRange { get { return m_SightRange; } set { m_SightRange = value; } }
        [DataMember]
        public float LeftAngle
        {
            get;
            set;
        }
        [DataMember]
        public bool Firing
        {
            get;
            set;
        }
        [IgnoreDataMember]
        private float m_Spread;//spread of the bullets
        [DataMember]
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
        [DataMember]
        public int NumberOfBullets
        {
            get;
            set;
        }
        [DataMember]
        //number of frames to wait between the end of one animation to the next
        public int FireRate
        {
            get;
            set;
        }
        protected static Random WEAPON_RANDOM = new Random();
        [DataMember]
        public int m_ElapsedFrames { get; set; }
        [DataMember]
        public bool CanDamage { get; set; }
        public Weapon() 
        {
        }
        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
        }
        //this should be called every update if it exists for the player
        public virtual void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, bool isFireDown)
        {
            //decrement unless its ready to fire or is being fired
            if (m_ElapsedFrames > 0 && !Firing)
            {
                m_ElapsedFrames -= 1;
            }
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
            if (m_ElapsedFrames == 0)
            {
                return true;
            }
            return false;
        }

        public virtual void LoadWeapon(Microsoft.Xna.Framework.Content.ContentManager content)
        {
        }

        protected virtual void LoadTextures(Microsoft.Xna.Framework.Content.ContentManager content)
        {
        }
    }
}
