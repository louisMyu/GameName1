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
        private ShotInfo m_SavedShotInfo;
        [IgnoreDataMember]
        private ShotInfo m_CurrentShotInfo;

        [DataMember]
        public ShotInfo SavedShotInfo { get { return m_SavedShotInfo; } set { m_SavedShotInfo = value; } }
        [DataMember]
        public ShotInfo CurrentShotInfo { get { return m_CurrentShotInfo; } set { m_CurrentShotInfo = value; } }
        public Shotgun(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 3;
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line(content));
            }
            FireRate = 15;
            blastString = "Shotgun-Blast-1";
            blast2String = "Shotgun-Blast-2";
            blast3String = "Shotgun-Blast-3";
            blast4String = "Shotgun-Blast-4";
            blast = content.Load<Texture2D>(blastString);
            blast2 = content.Load<Texture2D>(blast2String);
            blast3 = content.Load<Texture2D>(blast3String);
            blast4 = content.Load<Texture2D>(blast4String);
            Knockback = 250f;
        }
        public Shotgun()
        {
        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, int weaponLength)
        {
            base.Update(elapsedTime, playerCenter, rotationAngle, accuracy, weaponLength);
            if (!Firing)
            {
                float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
                //TODO: add a random so its either plus or minus accuracy
                float centerVector = rotationAngle - accuracyInRadians;

                float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
                LeftAngle = leftAngle;
                SightRange = weaponLength;
                m_CurrentShotInfo = new ShotInfo(playerCenter, rotationAngle, NumberOfBullets, leftAngle, 10);
            }
        }
        public override bool CheckCollision(GameObject ob, out Vector2 intersectingAngle)
        {
            foreach (Line line in m_BulletLines)
            {
                Vector2 check = line.Intersects(ob.m_Bounds);
                if (check.X != -1)
                {
                    intersectingAngle = new Vector2(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y); ;
                    return true;
                }
            }
            intersectingAngle = new Vector2(0, 0);
            return false;
        }
        public override void DrawWeapon(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            
        }

        public override void DrawBlast(SpriteBatch _spriteBatch, Vector2 position, float rot)
        {
            //firing a shot, save the state
            if (!Firing)
            {
                Firing = true;
                m_SavedShotInfo = m_CurrentShotInfo;
            }
            if (m_SavedShotInfo.NumFrames > 0)
            {
                float leftAngle = LeftAngle;
                foreach (Line line in m_BulletLines)
                {
                    line.Update(position, leftAngle, SightRange);
                    leftAngle += (float)(Spread / (NumberOfBullets - 1));
                }
                //foreach (Line line in m_BulletLines)
                //{
                //    line.Draw(_spriteBatch);
                //}
                if (m_SavedShotInfo.NumFrames > 7)
                {
                    _spriteBatch.Draw(blast, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                }
                else if (m_SavedShotInfo.NumFrames > 5)
                {
                    _spriteBatch.Draw(blast2, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                }
                else if (m_SavedShotInfo.NumFrames > 2)
                {
                    _spriteBatch.Draw(blast3, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                }
                else if (m_SavedShotInfo.NumFrames > 0)
                {
                    _spriteBatch.Draw(blast4, position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0, blast.Height / 2), 1.0f, SpriteEffects.None, 0f);
                }
                --m_SavedShotInfo.NumFrames;
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
