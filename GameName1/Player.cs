using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameName1
{
    class Player : GameObject
    {
        static float VELOCITY = 10.0F;
        private int SIGHT_RANGE = 120;
        private float FIRE_RATE = 1.0f;

        public Vector2 m_MoveToward = new Vector2();
        public int Life = 0;
		private Weapon testWeapon;
        private bool m_Moving = false;
        bool shotHappened = false;

        public Player() : base()
        {
			
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 pos)
        {
            testWeapon = new Shotgun(content);
            Position = pos;
        }
        public void CheckCollisions(List<GameObject> exists, List<GameObject> toRemove, out bool reset)
        {
            reset = false;
            bool deleteThisBoolLater = false;
            bool weaponHit;
            float nearestLength = float.MaxValue;
            shotHappened = false;
            foreach (GameObject ob in exists)
            {
                //TODO: seriously need to refactor this later
                //its good to find the nearest zombie when i run through entire zombie list, but probably not here
                Vector2 vec = new Vector2(ob.Position.X - Position.X, ob.Position.Y - Position.Y);
                float temp = vec.LengthSquared();
                if (temp < nearestLength && ob is Zombie)
                {
                    nearestLength = temp;
                    RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
                }
                if (ob == null)
                {
                    return;
                }
                weaponHit = testWeapon.CheckCollision(ob);
                if (weaponHit)
                {
                    if (testWeapon.CanFire() || shotHappened)
                    {
                        shotHappened = true;
                        if (ob is Zombie)
                        {
                            toRemove.Add(ob);
                        }
                        //continue here in case ob overlaps with weapon and player
                        continue;
                    }
                }
                if (ob.Bounds.Intersects(this.Bounds))
                {
                    if (ob is Zombie)
                    {
                        reset = true;
                    }
                    if (ob is PowerUp)
                    {
                        deleteThisBoolLater = true;
                        toRemove.Add(ob);
                    }
                }
            }
            //for now hitting the powerup will reset the game
            if (deleteThisBoolLater)
            {
                foreach (GameObject g in exists)
                {
                    if (g is Zombie)
                        toRemove.Add(g);
                }
                Game1.ZombiesSpawned = false;
                Game1.GameTimer = 0;
                Game1.itemMade = false;
            }
            foreach (GameObject g in toRemove)
            {
                exists.Remove(g);
            }
        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = content.Load<Texture2D>("Player");
            base.LoadContent(content);
        }

        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc)
        {
            //get a normalized direction toward the point
            Vector2 amount = new Vector2(loc.X - Position.X, loc.Y - Position.Y);
            if (amount.LengthSquared() > VELOCITY*VELOCITY)
            {
                amount = Vector2.Normalize(amount);
                amount *= VELOCITY;
            }
            base.Move(amount);
            //Position.X = (float)Math.Floor(Position.X);
            //Position.Y = (float)Math.Floor(Position.Y);
            Position.X = MathHelper.Clamp(Position.X, Width/2, Game1.GameWidth - (Width / 2));
            Position.Y = MathHelper.Clamp(Position.Y, Height/2, Game1.GameHeight - (Height / 2) - UI.OFFSET);

            Bounds.X = (int)Position.X - Width / 2;
            Bounds.Y = (int)Position.Y - Height / 2;
        }

        internal void ProcessInput(Vector2 vec)
        {      
            if (vec.X != -1)
            {
                m_Moving = true;
                m_MoveToward = vec;
            }
            else if (vec.X == Position.X && vec.Y == Position.Y)
            {
                m_Moving = false;
            }

            //test.Update(m_MoveToward, new Vector2(Position.X, Position.Y));
            if (m_Moving)
            {
                Move(m_MoveToward);
            }
        }

        public void Update(float elapsedTime)
        {
            testWeapon.Update(elapsedTime, Position, RotationAngle, 10, SIGHT_RANGE);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            base.Draw(_spriteBatch);

            if (shotHappened || testWeapon.Firing)
            {
                testWeapon.DrawBlast(_spriteBatch, Position, RotationAngle);
            }
        }
    }
}
