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
        private static const Random SlimeRandom = new Random();
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
        private float m_Speed = 1.0f;
        [DataMember]
        public float Speed { get { return m_Speed; } set { m_Speed = value; } }

        [IgnoreDataMember]
        public Body _circleBody;

        [DataMember]
        public int LifeTotal { get; set; }

        private MotionState m_State;
        [DataMember]
        public MotionState State { get; set; }

        private List<SlimeTrailPiece> m_SlimeTrailList;
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
            m_State = MotionState.Wandering;

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
        public override void Move(Microsoft.Xna.Framework.Vector2 loc)
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

            ////get a normalized direction toward the point that was passed in, probably the player
            //Vector2 vec = new Vector2(loc.X - Position.X, loc.Y - Position.Y);
            //if (vec.LengthSquared() <= (275.0f * 275.0f))
            //{
            //    m_State = MotionState.Locked;
            //}

            //switch (m_State)
            //{
            //    case MotionState.Wandering:
            //        if (RANDOM_GENERATOR.Next(150) % 150 == 1)
            //        {
            //            RotationAngle = (float)RANDOM_GENERATOR.NextDouble() * MathHelper.Pi * 2;
            //            m_Direction.X = (float)Math.Cos(RotationAngle);
            //            m_Direction.Y = (float)Math.Sin(RotationAngle);
            //        }
            //        break;

            //    case MotionState.Locked:
            //        m_Direction = vec;
            //        RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
            //        m_State = MotionState.Locked;
            //        m_Speed = 1.0f;
            //        break;
            //}
            //choose a cardinal direction or dont move
            int dir = SlimeRandom.Next(5);
            Vector2 vec = new Vector2(0, 0);
            switch (dir)
            {
                case 0:
                    vec.X = 1;
                    break;
                case 1:
                    vec.X = -1;
                    break;
                case 2:
                    vec.Y = 1;
                    break;
                case 3:
                    vec.Y = -1;
                    break;
                default:
                    break;
            }
            m_Direction = Vector2.Normalize(m_Direction);
            Vector2 amount = m_Direction * m_Speed;
            base.Move(amount);

            //Later on, remove the clamp to the edge and despawn when too far out of the screen.
            //Position.X = MathHelper.Clamp(Position.X, Width + UI.OFFSET, Game1.GameWidth - (Width / 2));
            //Position.Y = MathHelper.Clamp(Position.Y, Height, Game1.GameHeight - (Height / 2));
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }

            m_Bounds.X = (int)Position.X - Width / 2;
            m_Bounds.Y = (int)Position.Y - Height / 2;
        }
        public override void Update(Player player)
        {
            foreach (SlimeTrailPiece piece in m_SlimeTrailList)
            {
                piece.Update();
            }
            Move(player.Position);
            bodyPosition = _circleBody.Position;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 temp = ConvertUnits.ToDisplayUnits(_circleBody.Position);
            spriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, RotationAngle, m_Origin, 1.0f, SpriteEffects.None, 0f);
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
            Texture2D m_Texture;
            float m_Rotation;
            Rectangle m_Bounds;
            //time left in frames to exist
            int m_Life;
            bool isAlive;
            public SlimeTrailPiece(Rectangle rec, int life, Texture2D texture, float rot)
            {
                m_Rotation = rot;
                m_Bounds = new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
                m_Life = 100;
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
                _spriteBatch.Draw(m_Texture, m_Bounds, null, Color.White, m_Rotation, new Vector2(m_Bounds.Width / 2, m_Bounds.Height / 2), SpriteEffects.None, 0);
            }
        }
    }
}
