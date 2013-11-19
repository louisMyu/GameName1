using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace GameName1
{
    class Player : GameObject
    {
        static float VELOCITY = 5.0F;
        private int SIGHT_RANGE = 100;

        public Vector2 m_MoveToward = new Vector2();
        Vector3 a;
        public int Life = 0;
		private Weapon testWeapon;
        private bool m_Moving = false;
        bool shotHappened = false;

        Texture2D AlphaTexture;
        public int LifeTotal;

        public Magic WeaponSlot1Magic;
        public Magic WeaponSlot2Magic;

        public bool isFireButtonDown
        {
            get;
            set;
        }
        public Player() : base()
        {
			
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 pos)
        {
            testWeapon = new Shotgun(content);
            Position = pos;
            isFireButtonDown = false;
            LifeTotal = 100;
        }
        public void CheckCollisions(List<GameObject> exists, List<Zombie> zombies, out bool reset, World _world)
        {
            List<GameObject> removedAtEnd = new List<GameObject>();
            bool weaponHit = false;
            float nearestLength = float.MaxValue;
            shotHappened = false;
            reset = false;
            foreach (GameObject ob in exists)
            {
                //TODO: seriously need to refactor this later
                //its good to find the nearest zombie when i run through entire zombie list, but probably not here
                Vector2 vec = new Vector2(ob.Position.X - Position.X, ob.Position.Y - Position.Y);
                float temp = vec.LengthSquared();
                if (temp < nearestLength && ob is Zombie)
                {
                    nearestLength = temp;
                    if (!testWeapon.Firing)
                    {
                        RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
                    }
                }
                if (ob == null)
                {
                    //need to handle null exeception here
                    return;
                }
                if (ob.Bounds.Intersects(this.Bounds))
                {
                    if (ob is Zombie)
                    {
                        removedAtEnd.Add(ob);
                        zombies.Remove((Zombie)ob);
                        ((Zombie)ob).CleanBody();
                        LifeTotal -= 5;
                        if (LifeTotal <= 0)
                        {
                            reset = true;
                            LifeTotal = 100;
                            return;
                        }
                    }
                    if (ob is PowerUp)
                    {
                        if (WeaponSlot1Magic == null)
                        {
                            WeaponSlot1Magic = Magic.GetMagicType(ob);
                        }
                        removedAtEnd.Add(ob);
                    }
                }
            }

            foreach (GameObject g in removedAtEnd)
            {
                exists.Remove(g);
            }
            //TODO: seriously need to refactor this later
            //its good to find the nearest zombie when i run through entire zombie list, but probably not here
            if ((testWeapon.CanFire() && isFireButtonDown) || testWeapon.Firing)
            {
                if (!testWeapon.Firing)
                {
                    shotHappened = true;
                }
                foreach (GameObject ob in exists)
                {
                    Vector2 hitAngle = new Vector2();
                    weaponHit = testWeapon.CheckCollision(ob, out hitAngle);
                    if (weaponHit)
                    {
                        if (ob is Zombie)
                        {
                            Zombie temp = (Zombie)ob;
                            temp.LifeTotal -= 5;
                            if (temp.LifeTotal <= 0)
                            {
                                temp.CleanBody();
                                zombies.Remove((Zombie)ob);
                                removedAtEnd.Add(ob);
                            }
                            else
                            {
                                temp.ApplyLinearForce(hitAngle, testWeapon.Knockback);
                            }
                        }
                    }
                    weaponHit = false;
                }
            }
            foreach (GameObject g in removedAtEnd)
            {
                exists.Remove(g);
            }
        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = content.Load<Texture2D>("Player");
            AlphaTexture = content.Load<Texture2D>("TransparentSquare");
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
            Position.X = MathHelper.Clamp(Position.X, 0 + UI.OFFSET, Game1.GameWidth + Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, Game1.GameHeight + Height);

            Bounds.X = (int)Position.X - Width / 2;
            Bounds.Y = (int)Position.Y - Height / 2;
        }

        internal void ProcessInput(Vector2 vec, bool inPlayField, bool stopPlayer = false)
        {
            if (vec.X != -1 && Input.CurrentState == Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed && inPlayField)
            {
                m_Moving = true;
                m_MoveToward = vec;
            }
            if (stopPlayer)
            {
                m_Moving = false;
            }
            //test.Update(m_MoveToward, new Vector2(Position.X, Position.Y));
            
        }

        public void Update(float elapsedTime)
        {
            testWeapon.Update(elapsedTime, Position, RotationAngle, 10, SIGHT_RANGE);
            if ((m_MoveToward.X == Position.X && m_MoveToward.Y == Position.Y))
            {
                m_Moving = false;
            }
            if (m_Moving)
            {
                Move(m_MoveToward);
            }
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            base.Draw(_spriteBatch);
            _spriteBatch.Draw(AlphaTexture, Position, null, Color.White, RotationAngle, Origin, 1.0f, SpriteEffects.None, 0f);
            if (shotHappened || testWeapon.Firing)
            {
                testWeapon.DrawBlast(_spriteBatch, Position, RotationAngle);
            }
        }
    }
}
