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

namespace GameName1
{
    [KnownType(typeof(Slime))]
    [KnownType(typeof(GameObject))]
    [DataContract]
    public class Slime : GameObject, IEnemy
    {
        private const int DAMAGE_AMOUNT = 5;
        private static Random SlimeRandom = new Random();
        //number of frames to skip between adding a slime piece
        private const int SLIME_TRAIL_SKIP_TIME = 5;
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

        private List<SlimeTrailPiece> m_SlimeTrailList;
        private int m_SlimeTrailTimeCounter = 0;
        public Slime()
            : base()
        {
            LifeTotal = 40;

        }

        private Texture2D m_SlimeTrailTex;
        public void LoadContent(World world)
        {
            if (m_Texture == null)
            {
                m_Texture = TextureBank.GetTexture("Slime");
            }
            if (m_SlimeTrailTex == null)
            {
                m_SlimeTrailTex = TextureBank.GetTexture("SlimeTrail");
            }
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

            m_SlimeTrailList = new List<SlimeTrailPiece>();
            _circleBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(35 / 2f), 1f, ConvertUnits.ToSimUnits(Position));
            _circleBody.BodyType = BodyType.Dynamic;
            _circleBody.Mass = 0.2f;
            _circleBody.LinearDamping = 2f;
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
            temp.X = MathHelper.Clamp(Position.X, 0 + UI.OFFSET, Game1.GameWidth - Width/2);
            temp.Y = MathHelper.Clamp(Position.Y, 0, Game1.GameHeight - Height/2);
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
            m_SlimeTrailList.RemoveAll(x => !x.isAlive);
            foreach (SlimeTrailPiece piece in m_SlimeTrailList)
            {
                piece.Update();
            }
            Move(player.Position, elapsedTime);
            ++m_SlimeTrailTimeCounter;
            if (m_SlimeTrailTimeCounter % SLIME_TRAIL_SKIP_TIME == 0)
            {
                AddSlimePiece();
                m_SlimeTrailTimeCounter = 0;
            }
            bodyPosition = _circleBody.Position;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (SlimeTrailPiece p in m_SlimeTrailList)
            {
                p.Draw(spriteBatch);
            }
            spriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, RotationAngle, m_Origin, 1.0f, SpriteEffects.None, 0f);
        }
        private void AddSlimePiece()
        {
            Rectangle pieceRec = new Rectangle((int)Position.X, (int)Position.Y, (int)m_Texture.Width / 2, (int)m_Texture.Width / 2);
            SlimeTrailPiece piece = new SlimeTrailPiece(pieceRec, 100, m_SlimeTrailTex, RotationAngle);
            m_SlimeTrailList.Add(piece);
        }
        public void ApplyLinearForce(Vector2 angle, float amount)
        {
            Vector2 impulse = Vector2.Normalize(angle) * amount;
            _circleBody.ApplyLinearImpulse(impulse);
        }

        public void CleanBody()
        {
            if (_circleBody != null)
            {
                Game1.m_World.RemoveBody(_circleBody);

            }
        }
        public void AddToHealth(int amount)
        {
            LifeTotal += amount;
        }
        public int GetHealth()
        {
            return LifeTotal;
        }
        public int GetDamageAmount()
        {
            return DAMAGE_AMOUNT;
        }
        public override void Save()
        {
            Storage.Save<Slime>("", "", this);
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
        private class SlimeTrailPiece
        {
            private const int LIFE_TIME = 500;
            Texture2D m_Texture;
            float m_Rotation;
            Rectangle m_Bounds;
            //time left in frames to exist
            int m_Life;
            public bool isAlive;
            public SlimeTrailPiece(Rectangle rec, int life, Texture2D texture, float rot)
            {
                m_Texture = texture;
                m_Rotation = rot;
                m_Bounds = new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
                m_Life = 500;
                isAlive = true;
            }
            public void Update()
            {
                if (m_Life <= 0)
                {
                    isAlive = false;
                }
                else
                {
                    --m_Life;
                }
            }
            public void Draw(SpriteBatch _spriteBatch)
            {

                float temp;
                float alpha;
                if (m_Life / LIFE_TIME > 0.5)
                {
                    temp = 0.5f;
                }
                else
                {
                    temp = m_Life;
                }
                alpha = (temp / LIFE_TIME);
                _spriteBatch.Draw(m_Texture, m_Bounds, null, Color.White * alpha, m_Rotation, new Vector2(m_Bounds.Width / 2, m_Bounds.Height / 2), SpriteEffects.None, 0);
            }
        }
    }
}
