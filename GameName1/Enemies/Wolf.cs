using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameName1.Enemies
{
    class Wolf : GameObject, IEnemy
    {
        private const int DAMAGE_AMOUNT = 5;
        private static Random SlimeRandom = new Random();
        public enum MotionState
        {
            Wandering,
            Locked,
            Dead
        }
        [DataMember]
        public Vector2 bodyPosition { get; set; }
        [IgnoreDataMember]
        static private Texture2D m_Texture = null;
        [IgnoreDataMember]
        private float m_Speed = 1.5f;
        [DataMember]
        public float Speed { get { return m_Speed; } set { m_Speed = value; } }

        [IgnoreDataMember]
        public Body _circleBody;

        [DataMember]
        public int LifeTotal { get; set; }

        [DataMember]
        public MotionState State { get; set; }
        public Wolf()
            : base()
        {
            LifeTotal = 40;

        }
        private WolfHand Lefthand;
        private WolfHand Righthand;

        private static Texture2D m_SlimeTrailTex;
        public void LoadContent(World world)
        {
            int dir = SlimeRandom.Next(4);
            m_Direction = new Vector2(0, 0);
            switch (dir)
            {
                case 0:
                    m_Direction.X = 1;
                    break;
                case 1:
                    m_Direction.X = -1;
                    break;
                case 2:
                    m_Direction.Y = 1;
                    break;
                case 3:
                    m_Direction.Y = -1;
                    break;
                default:
                    break;
            }

            Width = m_Texture != null ? m_Texture.Width : 0;
            Height = m_Texture != null ? m_Texture.Height : 0;

            if (m_Texture != null)
            {
                m_Bounds.Width = Width;
                m_Bounds.Height = Height;
                m_Bounds.X = (int)Position.X - Width / 2;
                m_Bounds.Y = (int)Position.Y - Height / 2;
                m_Origin.X = Width / 2;
                m_Origin.Y = Height / 2;
            }

            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 5f;
            _circleBody.LinearDamping = 3f;
            _circleBody.Restitution = .5f;
        }

        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc, TimeSpan elapsedTime)
        {
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

            GetDirection();
            RotationAngle = (float)Math.Atan2(m_Direction.Y, m_Direction.X);
            Vector2 amount = m_Direction * m_Speed;
            base.Move(amount, elapsedTime);
            Vector2 temp = new Vector2();
            temp.X = MathHelper.Clamp(Position.X, 0 + UI.OFFSET, Game1.GameWidth - Width / 2);
            temp.Y = MathHelper.Clamp(Position.Y, 0, Game1.GameHeight - Height / 2);
            Position = temp;
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }

            m_Bounds.X = (int)Position.X - Width / 2;
            m_Bounds.Y = (int)Position.Y - Height / 2;
        }
        private void GetDirection()
        {
            int check = SlimeRandom.Next(100);
            //choose a cardinal direction or dont move
            if (check == 0)
            {
                int dir = SlimeRandom.Next(4);
                m_Direction = new Vector2(0, 0);
                switch (dir)
                {
                    case 0:
                        m_Direction.X = 1;
                        break;
                    case 1:
                        m_Direction.X = -1;
                        break;
                    case 2:
                        m_Direction.Y = 1;
                        break;
                    case 3:
                        m_Direction.Y = -1;
                        break;
                    default:
                        break;
                }
            }
            m_Direction = Vector2.Normalize(m_Direction);

        }
        public override void Update(Player player, TimeSpan elapsedTime)
        {
            ObjectManager.GetCell(Position).Remove(this);
            Move(player.Position, elapsedTime);
            ObjectManager.GetCell(Position).Add(this);

            bodyPosition = _circleBody.Position;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, RotationAngle, m_Origin, 1.0f, SpriteEffects.None, 0f);
        }

        public static void LoadTextures()
        {
            if (m_Texture == null)
            {
                m_Texture = TextureBank.GetTexture("Slime");
            }
            if (m_SlimeTrailTex == null)
            {
                m_SlimeTrailTex = TextureBank.GetTexture("SlimeTrail");
            }
            //TODO load slime exploded textures here
        }
        #region IEnemy
        public void CleanBody()
        {
            if (_circleBody != null)
            {
                GameplayScreen.m_World.RemoveBody(_circleBody);
            }
        }
        public void ApplyLinearForce(Vector2 angle, float amount)
        {
            Vector2 impulse = Vector2.Normalize(angle) * amount;
            _circleBody.ApplyLinearImpulse(impulse);
        }
        public void AddToHealth(int amount)
        {
            LifeTotal += amount;
        }
        public List<Texture2D> GetExplodedParts()
        {
            return ExplodedParts;
        }
        protected override void LoadExplodedParts()
        {
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart1"));
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart2"));
            ExplodedParts.Add(TextureBank.GetTexture("SlimePart3"));
        }
        public int GetHealth()
        {
            return LifeTotal;
        }
        public int GetDamageAmount()
        {
            return DAMAGE_AMOUNT;
        }
        public void DoCollision(Player player)
        {
            player.LifeTotal -= GetDamageAmount();
            ObjectManager.RemoveObject(this);
        }
        #endregion
        #region Save/Load
        public override void Save()
        {
        }
        public override void Load(World world)
        {
            if (m_Texture == null)
            {
                m_Texture = TextureBank.GetTexture("Slime");
            }
            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 0.2f;
            _circleBody.LinearDamping = 2f;
            _circleBody.Position = bodyPosition;
        }
        #endregion
        class WolfHand : GameObject, IEnemy
        {
            [DataMember]
            public int LifeTotal { get; set; }
            private enum LeftOrRightHand
            {
                Left,
                Right
            }
            private LeftOrRightHand WhichHand;
            private WolfHand(LeftOrRightHand which)
            {
                if (which == LeftOrRightHand.Left)
                {
                    Texture = TextureBank.GetTexture("kevinZombie");
                }
                else
                {
                    Texture = TextureBank.GetTexture("kevinZombie");
                }
                WhichHand = which;
            }
            public static WolfHand[] MakeHands()
            {
                WolfHand[] hands = new WolfHand[2];
                hands[0] = new WolfHand(LeftOrRightHand.Left);
                hands[1] = new WolfHand(LeftOrRightHand.Right);

                return hands;
            }
            public void Update(Wolf wolfBody)
            {
                switch (WhichHand)
                {
                    case LeftOrRightHand.Left:
                        break;
                    case LeftOrRightHand.Right:
                        break;
                }
            }
            private void SetTexture(Texture2D tex)
            {
                Texture = tex;
            }
            public int GetHealth()
            {
                throw new NotImplementedException();
            }

            public void AddToHealth(int amount)
            {
                LifeTotal += amount;
            }

            public void ApplyLinearForce(Vector2 angle, float amount)
            {
                throw new NotImplementedException();
            }

            public void CleanBody()
            {
                throw new NotImplementedException();
            }

            public int GetDamageAmount()
            {
                return DAMAGE_AMOUNT;
            }

            public List<Texture2D> GetExplodedParts()
            {
                throw new NotImplementedException();
            }

            public void DoCollision(Player p)
            {
                p.LifeTotal -= GetDamageAmount();
                ObjectManager.RemoveObject(this);
            }
        }
    }

}
