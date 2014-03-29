using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;

namespace GameName1
{
    [KnownType(typeof(GameObject))]
    [KnownType(typeof(Shotgun))]
    [DataContract]
    public class Player : GameObject
    {
        [IgnoreDataMember]
        public static readonly string playerSaveDir = "playerDir";
        [IgnoreDataMember]
        static float VELOCITY = 5.0F;

        [IgnoreDataMember]
        public Vector2 m_MoveToward = new Vector2();
        [DataMember]
        public Vector2 MoveToward { get { return m_MoveToward; } set { m_MoveToward = value; } }
        [DataMember]
        public int Life { get; set; }
        [IgnoreDataMember]
        private Weapon m_Weapon {get;set;}
        [DataMember]
        public Weapon Weapon { get { return m_Weapon; } set { m_Weapon = value; } }
        [IgnoreDataMember]
        private bool m_Moving = false;
        [DataMember]
        public bool Moving { get { return m_Moving; } set { m_Moving = value; } }
        [IgnoreDataMember]
        private bool shotHappened = false;
        [DataMember]
        public bool ShotHappened { get { return shotHappened; } set { shotHappened = value; } }
        [DataMember]
        public int LifeTotal { get; set; }
        [DataMember]
        public Magic WeaponSlot1Magic { get; set; }
        [DataMember]
        public Magic WeaponSlot2Magic { get; set; }

        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public bool isFireButtonDown
        {
            get;
            set;
        }
        public Player() : base()
        {
			
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Player player)
        {
            m_Weapon = player.m_Weapon;
            Position = player.Position;
            isFireButtonDown = player.isFireButtonDown;
            LifeTotal = player.LifeTotal;
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 pos)
        {
            m_Weapon = new Rifle();
            Position = pos;
            isFireButtonDown = false;
            LifeTotal = 100;
        }
        public void CheckCollisions(out bool reset, World _world)
        {
            List<GameObject> removedAtEnd = new List<GameObject>();
            bool weaponHit = false;
            float nearestLength = float.MaxValue;
            shotHappened = false;
            reset = false;
            foreach (GameObject ob in ObjectManager.AllGameObjects)
            {
                //TODO: seriously need to refactor this later
                //its good to find the nearest zombie when i run through entire zombie list, but probably not here
                Vector2 vec = new Vector2(ob.Position.X - Position.X, ob.Position.Y - Position.Y);
                float temp = vec.LengthSquared();
                if (temp < nearestLength && ob is Zombie)
                {
                    nearestLength = temp;
                    if (!m_Weapon.Firing)
                    {
                        RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
                    }
                }
                if (ob == null)
                {
                    //need to handle null exeception here
                    return;
                }
                if (ob.m_Bounds.Intersects(this.m_Bounds))
                {
                    if (ob is Zombie)
                    {
                        removedAtEnd.Add(ob);
                        LifeTotal -= 5;
                        if (LifeTotal <= 0)
                        {
                            reset = true;
                            LifeTotal = 100;
                            return;
                        }
                    }
                    if (ob is Anubis)
                    {
                        removedAtEnd.Add(ob);
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
                ObjectManager.RemoveObject(g);
            }
            removedAtEnd.Clear();
            //TODO: seriously need to refactor this later
            //its good to find the nearest zombie when i run through entire zombie list, but probably not here
            if ((m_Weapon.CanFire() && isFireButtonDown) || m_Weapon.Firing)
            {
                if (!m_Weapon.Firing)
                {
                    shotHappened = true;
                }
                foreach (GameObject ob in ObjectManager.AllGameObjects)
                {
                    Vector2 hitAngle = new Vector2();
                    //this probably should check for collision only when firing
                    //that way the bullet lines wont update to the next person while a shot is going off

                    //right now checkcollision is not finding collisions correctly,
                   //and enemies are leaving behind invisible islands again.  this happened when i first ported
                    //farseer into here.  can look at commits to find a possible issue that i can resolve.
                    weaponHit = m_Weapon.CheckCollision(ob, out hitAngle);
                    if (weaponHit)
                    {
                        if (ob is Zombie)
                        {
                            Zombie temp = (Zombie)ob;
                            temp.LifeTotal -= 10;
                            if (temp.LifeTotal <= 0)
                            {
                                removedAtEnd.Add(ob);
                                ++Score;
                            }
                            else
                            {
                                temp.ApplyLinearForce(hitAngle, m_Weapon.Knockback);
                            }
                        }
                    }
                    weaponHit = false;
                }
            }
            foreach (GameObject g in removedAtEnd)
            {
                ObjectManager.RemoveObject(g);
            }
        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = content.Load<Texture2D>("Player");
            base.LoadContent(content);
            m_Weapon.LoadContent(content);
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
            Vector2 temp = new Vector2();
            temp.X = MathHelper.Clamp(Position.X, 0 + UI.OFFSET, Game1.GameWidth + Width);
            temp.Y = MathHelper.Clamp(Position.Y, 0, Game1.GameHeight + Height);
            Position = temp;
            m_Bounds.X = (int)Position.X - Width / 2;
            m_Bounds.Y = (int)Position.Y - Height / 2;
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
            m_Weapon.Update(elapsedTime, Position, RotationAngle, 10, isFireButtonDown);
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

            if (shotHappened || m_Weapon.Firing)
            {
                m_Weapon.DrawBlast(_spriteBatch, Position, RotationAngle);
            }
        }

        public override void Save()
        {
            Storage.Save<Player>(Player.playerSaveDir, "player1", this);
        }
        public override void Load(Microsoft.Xna.Framework.Content.ContentManager content, World world)
        {
            throw new NotImplementedException();
        }
        public static Player Load(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Player p = Storage.Load<Player>(Player.playerSaveDir, "player1");
            if (p != null)
            {
                p.m_Weapon.LoadWeapon(content);
            }
            return p;
        }
    }
}
