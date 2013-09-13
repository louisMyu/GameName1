using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Shotgun : Weapon
    {
        private Texture2D blast;

        private List<Line> m_BulletLines = new List<Line>();
        private ShotInfo m_SavedShotInfo;
        private ShotInfo m_CurrentShotInfo;
        public Shotgun(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 3;
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line(content));
            }
            FireRate = 4.0f;
            blast = content.Load<Texture2D>("Shotgun-Blast-1");
        }
        
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public override void Update(float elapsedTime, Vector2 playerCenter, float rotationAngle, int accuracy, int weaponLength)
        {
            base.Update(elapsedTime, playerCenter, rotationAngle, accuracy, weaponLength);
            float accuracyInRadians = WEAPON_RANDOM.Next(0, accuracy) * ((float)Math.PI / 180);
            //TODO: add a random so its either plus or minus accuracy
            float centerVector = rotationAngle - accuracyInRadians;

            float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
            foreach (Line line in m_BulletLines)
            {  
                line.Update(playerCenter, leftAngle, weaponLength);
                leftAngle += (float)(Spread / (NumberOfBullets - 1));
            }
            m_CurrentShotInfo = new ShotInfo(playerCenter, rotationAngle, NumberOfBullets, leftAngle, 5);
        }
        public override bool CheckCollision(GameObject ob)
        {
            base.CheckCollision(ob);
            foreach (Line line in m_BulletLines)
            {
                Vector2 check = line.Intersects(ob.Bounds);
                if (check.X != -1)
                {
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
            //firing a shot, save the state
            if (!Firing)
            {
                Firing = true;
                m_SavedShotInfo = m_CurrentShotInfo;
            }
            if (m_SavedShotInfo.NumFrames > 0)
            {
                //foreach (Line line in m_BulletLines)
                //{
                //    line.Draw(_spriteBatch);
                //}
                _spriteBatch.Draw(blast, m_SavedShotInfo.Position, null, Color.White, m_SavedShotInfo.Rotation, new Vector2(0,blast.Height/2), 1.0f, SpriteEffects.None, 0f);
                --m_SavedShotInfo.NumFrames;
            }
            if (Firing && m_SavedShotInfo.NumFrames == 0)
            {
                Firing = false;
            }
        }
    }
}
