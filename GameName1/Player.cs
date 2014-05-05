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
        public static float VELOCITY = 40f;
        public static int CheatEffectFrames;
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
        public CheatPowerUp WeaponSlot1Magic { get; set; }
        private List<Cheat> m_ActiveEffects = new List<Cheat>();
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public bool isFireButtonDown
        {
            get;
            set;
        }
        public bool IsStopDown { get; set; }
        public Body _circleBody;

        private Texture2D ReticuleTexture;
        private Texture2D AimCircleTexture;
        private Texture2D AimCircleRedTexture;
        public Texture2D RedFlashTexture;

        public bool DrawRedFlash;
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
            IsStopDown = false;
        }
        public void Init(Microsoft.Xna.Framework.Content.ContentManager content, Vector2 pos)
        {
            m_Weapon = new Shotgun();
            Position = pos;
            isFireButtonDown = false;
            MaxLife = 100;
            LifeTotal = MaxLife;
            IsStopDown = false;
            DrawRedFlash = false;
        }
        public void CheckCollisions(out bool reset, World _world)
        {
            //float nearestLength = float.MaxValue;
            DrawRedFlash = false;
            reset = false;
            List<List<GameObject>> objectsToCheck = ObjectManager.GetCellsOfRectangle(Bounds);
            foreach (List<GameObject> gameObjectList in objectsToCheck)
            {
                foreach (GameObject ob in gameObjectList)
                {
                    if (ob is Player) { continue; }
                    //TODO: seriously need to refactor this later
                    //its good to find the nearest zombie when i run through entire zombie list, but probably not here
                    //this code will find the nearest zombie for autoaim
                    //Vector2 vec = new Vector2(ob.Position.X - Position.X, ob.Position.Y - Position.Y);
                    //float temp = vec.LengthSquared();
                    //if (temp < nearestLength && ob is Zombie)
                    //{
                    //    nearestLength = temp;
                    //    if (!m_Weapon.Firing)
                    //    {
                    //        //RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
                    //    }
                    //}

                    if (ob == null)
                    {
                        //need to handle null exeception here
                        return;
                    }
                    if (ob.m_Bounds.Intersects(this.m_Bounds))
                    {
                        if (ob is IEnemy)
                        {
                            IEnemy enemy = ob as IEnemy;
                            LifeTotal -= enemy.GetDamageAmount();
                            DrawRedFlash = true;
                            if (LifeTotal <= 0)
                            {
                                reset = true;
                                LifeTotal = 100;
                                return;
                            }
                        }
                        if (ob is CheatPowerUp)
                        {
                            CheatPowerUp temp = ob as CheatPowerUp;
                            if (temp.CheatEffect is IInstant)
                            {
                                IInstant instantEffect = temp.CheatEffect as IInstant;
                                instantEffect.GetInstantEffect();
                                WeaponSlot1Magic = null;
                            }
                            else if (WeaponSlot1Magic == null)
                            {
                                CheatPowerUp cheat = ob as CheatPowerUp;
                                WeaponSlot1Magic = cheat;
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
            }
            if (m_Weapon.Firing || m_Weapon.BulletsExist)
            {

                foreach (GameObject ob in ObjectManager.AllGameObjects)
                {
                    if (ob is PowerUp)
                    {
                        continue;
                    }
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
            ReticuleTexture = TextureBank.GetTexture("Reticule");
            AimCircleTexture = TextureBank.GetTexture("AimRing");
            AimCircleRedTexture = TextureBank.GetTexture("AimRingRed");
            RedFlashTexture = TextureBank.GetTexture("RED");
        }

        private bool KickedBack = false;
        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc, TimeSpan elapsedTime)
        {
            
            if (Input.UseAccelerometer)
            {
                base.Move(loc, elapsedTime);
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
                base.Move(amount, elapsedTime);
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
            IsStopDown = stopMoving;
            //test.Update(m_MoveToward, new Vector2(Position.X, Position.Y));
        }

        public void Update(TimeSpan elapsedTime)
        {
            //must be a better way to do this...
            foreach (Cheat cheat in m_ActiveEffects)
            {
                if (cheat.IsDone())
                {
                    cheat.EndEffect(this);
                }
            }
            m_ActiveEffects.RemoveAll(x => x.IsDone() || x == null);
            foreach (Cheat cheat in m_ActiveEffects)
            {
                if (cheat == null) continue;
                cheat.Update(this);
            }

            if (!KickedBack && isFireButtonDown && m_Weapon.CanFire())
            {
                KickedBack = true;
                if (m_Weapon is Shotgun)
                {
                    Vector2 temp = new Vector2((float)Math.Cos(RotationAngle), (float)Math.Sin(RotationAngle)) * -50;
                    this._circleBody.ApplyLinearImpulse(temp);
                }
            }
            if (!m_Weapon.Firing && KickedBack)
            {
                KickedBack = false;
            }
            ObjectManager.GetCell(Position).Remove(this);
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
                Vector2 acceleration = new Vector2(Input.CurrentAccelerometerValues.X, Input.CurrentAccelerometerValues.Y);
                if (acceleration.LengthSquared() > Input.Tilt_Threshold)
                {
                    m_MoveToward = new Vector2(MathHelper.Clamp(acceleration.X * 30, -(Math.Abs(acceleration.X) * VELOCITY), Math.Abs(acceleration.X) * VELOCITY),
                                                -1 * MathHelper.Clamp(acceleration.Y * 30, -(Math.Abs(acceleration.Y) * VELOCITY), Math.Abs(acceleration.Y) * VELOCITY));
                    if (!m_Weapon.Firing)
                    {
                        //dont apply rotation unless tilt amount is greater than a threshold
                        //if (!IsStopDown)
                        //{
                        //    RotationAngle = (float)Math.Atan2(-acceleration.Y, acceleration.X);
                        //}
                        //RotationAngle = UI.ThumbStickAngle;
                    }
                }
                else
                {
                    m_MoveToward = new Vector2(0, 0);
                }
                if (!m_Weapon.Firing)
                {
                    RotationAngle = UI.ThumbStickAngle;
                }
            }
            if (m_Moving && m_Weapon.Firing && m_Weapon.CanMoveWhileShooting)
            {
                Move(m_MoveToward, elapsedTime);
            }
            else if (m_Moving && !m_Weapon.Firing)
            {
                Move(m_MoveToward, elapsedTime);
            }
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }
            ObjectManager.GetCell(Position).Add(this);
            Vector2 playerVel = m_Moving ? m_MoveToward : new Vector2(0, 0);
            m_Weapon.Update(Position, playerVel, RotationAngle, 10, isFireButtonDown, elapsedTime);
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            if (IsStopDown)
            {
                Vector2 aimScale = Utilities.GetSpriteScaling(new Vector2(UI.StopButtonRec.Width, UI.StopButtonRec.Height), new Vector2(AimCircleTexture.Width, AimCircleTexture.Height));
                Texture2D temp;
                if (UI.ThumbStickPointOffset.LengthSquared() > (UI.StopButtonRec.Width / 2) * (UI.StopButtonRec.Width / 2))
                {
                    temp = AimCircleRedTexture;
                }
                else
                {
                    temp = AimCircleTexture;
                }
                _spriteBatch.Draw(temp, Position, null, Color.White, 0.0f, new Vector2(AimCircleTexture.Width / 2, AimCircleTexture.Height / 2), aimScale, SpriteEffects.None, 0);
            }
            base.Draw(_spriteBatch);
            
            m_Weapon.DrawBlast(_spriteBatch, Position, RotationAngle);
            if (IsStopDown)
            {
                _spriteBatch.Draw(ReticuleTexture, Position + UI.ThumbStickPointOffset, null, Color.White, 0.0f, new Vector2(7, 7), 1.0f, SpriteEffects.None, 0);
            }
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
        public void StartCheatEffect()
        {
            if (WeaponSlot1Magic != null)
            {
                m_ActiveEffects.Add(WeaponSlot1Magic.CheatEffect);
                WeaponSlot1Magic.CheatEffect.StartEffect(this);
                WeaponSlot1Magic = null;
            }
        }
    }
}
