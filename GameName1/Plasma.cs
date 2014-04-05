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
        [DataMember]
        public string shotString1 { get; set; }
        [DataMember]
        public string shotString2 { get; set; }
        
        [IgnoreDataMember]
        private SpriteInfo m_SavedShotInfo;
        [IgnoreDataMember]
        private SpriteInfo m_CurrentShotInfo;

        [DataMember]
        public SpriteInfo SavedShotInfo { get { return m_SavedShotInfo; } set { m_SavedShotInfo = value; } }
        [DataMember]
        public SpriteInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }

        public Plasma()
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 1;
            FireRate = 15;
            m_SightRange = 400;
            Knockback = 250f;
            CanMoveWhileShooting = true;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            LoadTextures(content);
        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, bool shotFired)
        {
            base.Update(elapsedTime, playerCenter, rotationAngle, accuracy, shotFired);
            if (!Firing)
            {
                //float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
                //TODO: add a random so its either plus or minus accuracy
                float centerVector = rotationAngle;
                if (NumberOfBullets > 1)
                {
                    float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
                    LeftAngle = leftAngle;
                }
                else
                {
                    LeftAngle = centerVector;
                }
                
                //foreach (Line line in m_BulletLines)
                //{
                //    line.Update(playerCenter, LeftAngle, SightRange);
                //}
                m_CurrentShotInfo = new SpriteInfo(playerCenter, rotationAngle, NumberOfBullets, LeftAngle);
            }
            //firing a shot, save the state
            if (!Firing && shotFired && CanFire())
            {
                Firing = true;
                CanDamage = false;
            }
        }
        public override bool CheckCollision(GameObject ob)
        {
            if (!CanDamage)
            {
                return false;
            }
            //foreach (Line line in m_BulletLines)
            //{
            //    Vector2 check = line.Intersects(ob.m_Bounds);
            //    if (check.X != -1)
            //    {
            //        Vector2 intersectingAngle = new Vector2(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y);
            //        IEnemy enemy = ob as IEnemy;
            //        enemy.ApplyLinearForce(intersectingAngle, Knockback);
            //        return true;
            //    }
            //}
            return false;
        }
        public override void DrawWeapon(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {

        }

        public override void DrawBlast(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            throw new NotImplementedException();
            //if (m_FireAnimation.CanDraw())
            //{
            //}
            //else if (Firing)
            //{
            //    Firing = false;
            //    m_ElapsedFrames = FireRate;
            //}
        }
        public override void LoadWeapon(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            LoadTextures(content);

            //m_BulletLines = new List<Line>();
            //for (int i = 0; i < NumberOfBullets; ++i)
            //{
            //    m_BulletLines.Add(new Line(content));
            //}
        }
        protected override void LoadTextures(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Bullet s = new Bullet("PlasmaBullet", content);
        }   
    }
    public class Bullet : GameObject
    {
        public Bullet(string s, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = TextureBank.GetTexture(s, content);
        }
    }
}
