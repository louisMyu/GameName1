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
        //spread of the bullets
        public float Spread
        {
            get;
            set;
        }
        public int NumberOfBullets
        {
            get;
            set;
        }

        private List<Line> m_BulletLines = new List<Line>();
        public Shotgun(Microsoft.Xna.Framework.Content.ContentManager device)
        {
            Spread = (float)Math.PI / 6;
            NumberOfBullets = 3;
            for (int i = 0; i < NumberOfBullets; ++i)
            {
                m_BulletLines.Add(new Line(device));
            }
        }
        
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = content.Load<Texture2D>("Shotgun");
            base.LoadContent(content);

        }
        //foreach line of the shotgun i need to update the lines based on the player center,
        //and rotate it and give it length, then update the graphical lines
        public void Update(Vector2 playerCenter, float rotationAngle, int accuracy, int weaponLength)
        {
            float accuracyInRadians = RANDOM_GENERATOR.Next(0, accuracy) * ((float)Math.PI / 180);
            //TODO: add a random so its either plus or minus accuracy
            float centerVector = rotationAngle - accuracyInRadians;

            float leftAngle = centerVector - (Spread / (NumberOfBullets - 1));
            foreach (Line line in m_BulletLines)
            {
                line.UpdateFromRotation(playerCenter, leftAngle,weaponLength);
                line.Update();
                leftAngle += (float)(Spread / (NumberOfBullets - 1));
            }
        }
        public bool CheckCollision(GameObject ob)
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
        public override void Draw(SpriteBatch _spriteBatch)
        {
            foreach (Line line in m_BulletLines)
            {
                line.Draw(_spriteBatch);
            }
        }
    }
}
