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
using FarseerPhysics.Factories;
using FarseerPhysics;

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
        [IgnoreDataMember]
        private Weapon m_Weapon {get;set;}
        [DataMember]
        public Weapon Weapon { get { return m_Weapon; } set { m_Weapon = value; } }
        [IgnoreDataMember]
        private bool m_Moving = false;
        [DataMember]
        public bool Moving { get { return m_Moving; } set { m_Moving = value; } }
        [DataMember]
        public int LifeTotal { get; set; }
        public int MaxLife { get; set; }
        [DataMember]
        public IMagic WeaponSlot1Magic { get; set; }

        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public bool isFireButtonDown
        {
            get;
            set;
        }
        public Body _circleBody;
        public Player() : base()
        {
			
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Player player)
        {
            m_Weapon = player.m_Weapon;
            Position = player.Position;
            isFireButtonDown = player.isFireButtonDown;
            LifeTotal = player.LifeTotal;
            MaxLife = player.MaxLife;
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 pos)
        {
            m_Weapon = new Plasma();
            Position = pos;
            isFireButtonDown = false;
            MaxLife = 100;
            LifeTotal = MaxLife;
        }
        public void CheckCollisions(out bool reset, World _world)
        {
            float nearestLength = float.MaxValue;

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
                        //RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
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
                    }
                    if (ob is CheatPowerUp)
                    {
                        if (WeaponSlot1Magic == null)
                        {
                            CheatPowerUp cheat = ob as CheatPowerUp;
                            if (cheat.CheatType == "Powerup")
                            {
                               
                            }
                        }
                    }
                    if (ob is WeaponPowerUp)
                    {
                        Weapon weapon = WeaponPowerUp.GetWeaponType((WeaponPowerUp)ob);
                        weapon.LoadContent();
                        m_Weapon = weapon;
                    }
                    ObjectManager.RemoveObject(ob);
                }
            }
            //TODO: seriously need to refactor this later
            //its good to find the nearest zombie when i run through entire zombie list, but probably not here
            if (m_Weapon.Firing || m_Weapon.BulletsExist)
            {
                if (!KickedBack && isFireButtonDown)
                {
                    KickedBack = true;
                    if (m_Weapon is Shotgun)
                    {
                        Vector2 temp = new Vector2((float)Math.Cos(RotationAngle), (float)Math.Sin(RotationAngle)) * -50;
                        this._circleBody.ApplyLinearImpulse(temp);
                    }
                }
                foreach (GameObject ob in ObjectManager.AllGameObjects)
                {
                    //this probably should check for collision only when firing
                    //that way the bullet lines wont update to the next person while a shot is going off
                    if (m_Weapon.CheckCollision(ob))
                    {
                        ++Score;
                    }
                }
            }
        }
        public void LoadContent(World world)
        {
            Texture = TextureBank.GetTexture("Player");
            base.LoadContent();
            m_Weapon.LoadContent();

            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 0.2f;
            _circleBody.LinearDamping = 2f;
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }
        }

        private bool KickedBack = false;
        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc)
        {
            
            if (Input.UseAccelerometer)
            {
                base.Move(loc);
                Vector2 temp = new Vector2();
                temp.X = MathHelper.Clamp(Position.X, 0 + UI.OFFSET, Game1.GameWidth);
                temp.Y = MathHelper.Clamp(Position.Y, 0, Game1.GameHeight);
                Position = temp;
                m_Bounds.X = (int)Position.X - Width / 2;
                m_Bounds.Y = (int)Position.Y - Height / 2;
            }
            else
            {
                //get a normalized direction toward the point
                Vector2 amount = new Vector2(loc.X - Position.X, loc.Y - Position.Y);
                if (amount.LengthSquared() > VELOCITY * VELOCITY)
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
        }

        internal void ProcessInput(bool firing, bool stopMoving)
        {
            isFireButtonDown = firing;
            m_Moving = !stopMoving;
            //test.Update(m_MoveToward, new Vector2(Position.X, Position.Y));
            
        }

        public void Update(float elapsedTime)
        {
            if (!m_Weapon.Firing && KickedBack)
            {
                KickedBack = false;
            }
            //should really just use the Sim's position for everything instead of converting from one to another
            Vector2 simPosition = ConvertUnits.ToDisplayUnits(_circleBody.Position);
            if (float.IsNaN(simPosition.X) || float.IsNaN(simPosition.Y))
            {
                return;
            }
            else
            {
                this.Position = simPosition;
            }
            if (!Input.UseAccelerometer)
            {
                if ((m_MoveToward.X == Position.X && m_MoveToward.Y == Position.Y))
                {
                    m_Moving = false;
                }
            }
            else
            {
                Vector3 acceleration = Input.CurrentAccelerometerValues;
                m_MoveToward = new Vector2(MathHelper.Clamp(acceleration.X*50, -(Math.Abs(acceleration.X)*25), Math.Abs(acceleration.X)*25),
                                            -1*MathHelper.Clamp(acceleration.Y*50, -(Math.Abs(acceleration.Y)*25), Math.Abs(acceleration.Y)*25));
                if (!m_Weapon.Firing)
                {
                    RotationAngle = (float)Math.Atan2(-acceleration.Y, acceleration.X);
                }
            }
            if (m_Moving && m_Weapon.Firing && m_Weapon.CanMoveWhileShooting && !KickedBack)
            {
                Move(m_MoveToward);
            }
            else if (m_Moving && !m_Weapon.Firing)
            {
                Move(m_MoveToward);
            }
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }
            Vector2 playerVel = m_Moving ? m_MoveToward : new Vector2(0, 0);
            m_Weapon.Update(elapsedTime, Position, playerVel, RotationAngle, 10, isFireButtonDown);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            base.Draw(_spriteBatch);
            m_Weapon.DrawBlast(_spriteBatch, Position, RotationAngle);
        }

        public override void Save()
        {
            Storage.Save<Player>(Player.playerSaveDir, "player1", this);
        }
        public static Player Load(Microsoft.Xna.Framework.Content.ContentManager content, World world)
        {
            Player p = Storage.Load<Player>(Player.playerSaveDir, "player1");
            if (p != null)
            {
                p.m_Weapon.LoadWeapon();
            }
            p._circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(p.Position));
            p._circleBody.BodyType = BodyType.Dynamic;
            p._circleBody.Mass = 0.2f;
            p._circleBody.LinearDamping = 2f;
            if (!float.IsNaN(p.Position.X) && !float.IsNaN(p.Position.Y))
            {
                p._circleBody.Position = ConvertUnits.ToSimUnits(p.Position);
            }
            return p;
        }
        public static Player Load(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Player p = Storage.Load<Player>(Player.playerSaveDir, "player1");
            if (p != null)
            {
                p.m_Weapon.LoadWeapon();
            }
            return p;
        }
    }
}
