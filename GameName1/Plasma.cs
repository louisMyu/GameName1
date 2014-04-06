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
    class Plasma : Weapon
    {     
        [IgnoreDataMember]
        private SpriteInfo m_SavedShotInfo;
        [IgnoreDataMember]
        private SpriteInfo m_CurrentShotInfo;

        [DataMember]
        public SpriteInfo SavedShotInfo { get { return m_SavedShotInfo; } set { m_SavedShotInfo = value; } }
        [DataMember]
        public SpriteInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }

        private List<Bullet> m_Bullets = new List<Bullet>();
        public Plasma() : base()
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 1;
            FireRate = 5;
            m_SightRange = 400;
            Knockback = 250f;
            CanMoveWhileShooting = true;
            Firing = false;
        }

        public override void LoadContent()
        {
            LoadTextures();
        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, bool shotFired)
        {
            base.Update(elapsedTime, playerCenter, rotationAngle, accuracy, shotFired);
            //float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
            //TODO: add a random so its either plus or minus accuracy
            float centerVector = rotationAngle;

            m_CurrentShotInfo = new SpriteInfo(playerCenter, rotationAngle, NumberOfBullets, LeftAngle);
            
            m_Bullets.RemoveAll(x => x.CanDelete);
            foreach (Bullet b in m_Bullets)
            {
                b.Update();
            }
            //firing a shot, save the state
            if (shotFired && CanFire())
            {
                m_Bullets.Add(new Bullet(TextureBank.GetTexture("PlasmaBullet"), m_CurrentShotInfo, 20));
                m_ElapsedFrames = FireRate;
            }
            if (m_Bullets.Count > 0)
            {
                BulletsExist = true;
            }
            else
            {
                BulletsExist = false;
            }
        }
        public override bool CheckCollision(GameObject ob)
        {
            bool hit = false;
            //i think im having an issue with bullets skipping their target because they are 
            //traveling too far per frame
            foreach (Bullet b in m_Bullets)
            {
                hit = b.CheckCollision(ob);
                if (hit)
                {
                    b.CanDelete = true;
                    if (ob is IEnemy)
                    {
                        IEnemy temp = ob as IEnemy;
                        temp.AddToHealth(-10);
                    }
                    return true;
                }
            }
            return false;
        }
        public override void DrawWeapon(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {

        }

        public override void DrawBlast(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            foreach (Bullet b in m_Bullets)
            {
                b.Draw(_spriteBatch);
            }
            //if (m_FireAnimation.CanDraw())
            //{
            //}
            //else if (Firing)
            //{
            //    Firing = false;
            //    m_ElapsedFrames = FireRate;
            //}
        }
        public override void LoadWeapon()
        {
            LoadTextures();

            //m_BulletLines = new List<Line>();
            //for (int i = 0; i < NumberOfBullets; ++i)
            //{
            //    m_BulletLines.Add(new Line(content));
            //}
        }
        protected override void LoadTextures()
        {
        }   
    }
    public class Bullet : GameObject
    {
        public int Velocity { get; set; }
        private Vector2 m_Heading;
        //time in frames that this bullet will exist
        private int Life;
        public Bullet(Texture2D tex, SpriteInfo info, int vel) : base()
        {
            Texture = tex;
            Position = info.Position;
            RotationAngle = info.Rotation;
            m_Heading = new Vector2((float)Math.Cos(RotationAngle), (float)Math.Sin(RotationAngle));
            Velocity = vel;
            Life = 25;
        }

        public void Update()
        {
            --Life;
            Move(Velocity * m_Heading);
            m_Bounds.X = (int)Position.X - Width / 2;
            m_Bounds.Y = (int)Position.Y - Height / 2;
        }
        public bool IsAlive()
        {
            return Life > 0;
        }
        public override void Draw(SpriteBatch _spriteBatch)
        {
            //base.Draw(_spriteBatch);
            _spriteBatch.Draw(Texture, new Vector2(Bounds.X, Bounds.Y), Color.White);
        }

        public bool CheckCollision(GameObject ob)
        {
            if (Bounds.Intersects(ob.Bounds))
            {
                return true;
            }
            return false;
        }
    }
}
