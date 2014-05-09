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
    public class Shotgun : Weapon
    {
        [DataMember]
        public string blastString { get; set; }
        [DataMember]
        public string blast2String { get; set; }
        [DataMember]
        public string blast3String { get; set; }
        [DataMember]
        public string blast4String { get; set; }
        [IgnoreDataMember]
        private List<Line> m_BulletLines = new List<Line>();
        [IgnoreDataMember]
        private SpriteInfo m_SavedShotInfo;
        [IgnoreDataMember]
        private SpriteInfo m_CurrentShotInfo;
        [DataMember]
        public SpriteInfo SavedShotInfo { get { return m_SavedShotInfo; } set { m_SavedShotInfo = value; } }
        [DataMember]
        public SpriteInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }

        private AnimationManager m_FireAnimation;
        public Shotgun() : base()
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 3;
            FireRate = 15;
            blastString = "Shotgun-Blast-1";
            blast2String = "Shotgun-Blast-2";
            blast3String = "Shotgun-Blast-3";
            blast4String = "Shotgun-Blast-4";
            SightRange = 100;
            Knockback = 250f;
            CanMoveWhileShooting = true;
            BulletsExist = false;
        }

        public override void LoadContent()
        {
            LoadTextures();
            LoadSounds();
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line());
            }
        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(Vector2 playerCenter, Vector2 playerVelocity, float rotationAngle, int accuracy, bool shotFired, TimeSpan elapsedTime)
        {
            base.Update(playerCenter, playerVelocity, rotationAngle, accuracy, shotFired, elapsedTime);
            if (!Firing)
            {
                float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
                //TODO: add a random so its either plus or minus accuracy
                float centerVector = rotationAngle - accuracyInRadians;

                float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
                LeftAngle = leftAngle;
                foreach (Line line in m_BulletLines)
                {
                    line.Update(playerCenter, leftAngle, SightRange);
                    leftAngle += (float)(Spread / (NumberOfBullets - 1));
                }
                m_CurrentShotInfo = new SpriteInfo(playerCenter, playerVelocity, rotationAngle, NumberOfBullets, leftAngle);
            }
            //firing a shot, save the state
            if (!Firing && shotFired && CanFire())
            {
                Firing = true;
                m_FireAnimation.SpriteInfo = m_CurrentShotInfo;
                CanDamage = true;
                if (m_FireAnimation.CanStartAnimating())
                    m_FireAnimation.Finished = false;
            }
        }
        //returns true if enemy died
        public override bool CheckCollision(GameObject ob)
        {
            if (!CanDamage)
            {
                return false;
            }
            foreach (Line line in m_BulletLines)
            {
                Vector2 check = line.Intersects(ob.m_Bounds);
                if (check.X != -1)
                {
                    Vector2 intersectingAngle = new Vector2(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y);
                    IEnemy enemy;
                    if ((enemy = ob as IEnemy) != null)
                    {
                        enemy.ApplyLinearForce(intersectingAngle, Knockback);
                        enemy.AddToHealth(-10);
                        if (enemy.GetHealth() <= 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public override void DrawWeapon(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            
        }

        public override void DrawBlast(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            if (m_FireAnimation.CanDraw() && Firing)
            {
                m_FireAnimation.DrawAnimationFrame(_spriteBatch);
                foreach (Line line in m_BulletLines)
                {
                    line.Draw(_spriteBatch);
                }
                //if frame is at 12
                if (m_FireAnimation.FrameCounter == 12)
                {
                    CanDamage = false;
                }
            }
            else if (Firing)
            {
                Firing = false;
                m_ElapsedFrames = FireRate;
            }
        }
        public override void LoadWeapon()
        {
            LoadTextures();
            m_BulletLines = new List<Line>();
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line());
            }
        }
        protected override void LoadTextures()
        {
            AnimationInfo[] array = new AnimationInfo[4];
            array[0] = new AnimationInfo(TextureBank.GetTexture(blastString), 5);
            array[1] = new AnimationInfo(TextureBank.GetTexture(blast2String), 9);
            array[2] = new AnimationInfo(TextureBank.GetTexture(blast3String), 12);
            array[3] = new AnimationInfo(TextureBank.GetTexture(blast4String), -1);
            m_FireAnimation = new AnimationManager(array, m_SavedShotInfo, 15);
        }

        protected override void LoadSounds()
        {
            m_ShotSound = 
        }
    }
}
