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
        [IgnoreDataMember]
        private Texture2D blast;
        [IgnoreDataMember]
        private Texture2D blast2;
        [IgnoreDataMember]
        private Texture2D blast3;
        [IgnoreDataMember]
        private Texture2D blast4;
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
<<<<<<< HEAD
        public SpriteInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }
=======
        public ShotInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014

        private AnimationManager m_FireAnimation;
        public Shotgun()
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 3;
            FireRate = 15;
            blastString = "Shotgun-Blast-1";
            blast2String = "Shotgun-Blast-2";
            blast3String = "Shotgun-Blast-3";
            blast4String = "Shotgun-Blast-4";

            Knockback = 250f;
        }

<<<<<<< HEAD
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            blast = TextureBank.GetTexture(blastString, content);
            blast2 = TextureBank.GetTexture(blast2String, content);
            blast3 = TextureBank.GetTexture(blast3String, content);
            blast4 = TextureBank.GetTexture(blast4String, content);
=======
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            blast = TextureBank.GetTexture(blastString, Content);
            blast2 = TextureBank.GetTexture(blast2String,Content);
            blast3 = TextureBank.GetTexture(blast3String, Content);
            blast4 = TextureBank.GetTexture(blast4String, Content);
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
            AnimationInfo[] array = new AnimationInfo[4];
            array[0].Texture = blast4;
            array[0].NextFrame = -1;
            array[1].Texture = blast3;
            array[1].NextFrame = 12;
            array[2].Texture = blast2;
            array[2].NextFrame = 9;
            array[3].Texture = blast;
            array[3].NextFrame = 5;
<<<<<<< HEAD
            m_FireAnimation = new AnimationManager(array, m_SavedShotInfo, 15);
=======
			m_FireAnimation = new m_FireAnimation(array, 
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line(content));
            }
        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, int weaponLength, bool shotFired)
        {
            base.Update(elapsedTime, playerCenter, rotationAngle, accuracy, weaponLength, shotFired);
            if (!Firing)
            {
                float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
                //TODO: add a random so its either plus or minus accuracy
                float centerVector = rotationAngle - accuracyInRadians;

                float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
                LeftAngle = leftAngle;
                SightRange = weaponLength;
                foreach (Line line in m_BulletLines)
                {
                    line.Update(playerCenter, LeftAngle, SightRange);
                    leftAngle += (float)(Spread / (NumberOfBullets - 1));
                }
<<<<<<< HEAD
                m_CurrentShotInfo = new SpriteInfo(playerCenter, rotationAngle, NumberOfBullets, leftAngle);
=======
                m_CurrentShotInfo = new ShotInfo(playerCenter, rotationAngle, NumberOfBullets, leftAngle, 15);
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
            }
            //firing a shot, save the state
            if (!Firing && shotFired && CanFire())
            {
                Firing = true;
<<<<<<< HEAD
                m_FireAnimation.SpriteInfo = m_CurrentShotInfo;
=======
                m_FireAnimation.ShotInfo = m_CurrentShotInfo;
>>>>>>> 1818f5996d12bbade7f4a5e8b45e5c942f130014
                CanDamage = true;
                if (m_FireAnimation.CanStartAnimating())
                    m_FireAnimation.Finished = false;
            }
        }
        public override bool CheckCollision(GameObject ob, out Vector2 intersectingAngle)
        {
            intersectingAngle = new Vector2(0, 0);
            if (!CanDamage)
            {
                return false;
            }
            foreach (Line line in m_BulletLines)
            {
                Vector2 check = line.Intersects(ob.m_Bounds);
                if (check.X != -1)
                {
                    intersectingAngle = new Vector2(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y); ;
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
            if (m_FireAnimation.CanDraw())
            {
                m_FireAnimation.DrawAnimationFrame(_spriteBatch);
                //if frame is at 5
                if (m_FireAnimation.CurrentFrame == 5)
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
        public override void LoadWeapon(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            blast = TextureBank.GetTexture(blastString, content);
            blast2 = TextureBank.GetTexture(blast2String, content);
            blast3 = TextureBank.GetTexture(blast3String, content);
            blast4 = TextureBank.GetTexture(blast4String, content);
            m_BulletLines = new List<Line>();
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line(content));
            }
        }
    }
}
