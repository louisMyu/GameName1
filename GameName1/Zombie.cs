using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace GameName1
{
    class Zombie : GameObject
    {
        private static Texture2D GLOBAL_TEXTURE = null;
        private float SPEED = 0.5f;

        public Body _circleBody;

        private enum State
        {
            Wandering,
            Locked,
            Dead
        }

        public int LifeTotal;

        private State m_State;
        public Zombie() : base()
        {
            LifeTotal = 50;
            
        }
        
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, World world)
        {
            if (GLOBAL_TEXTURE == null)
            {
                GLOBAL_TEXTURE = Content.Load<Texture2D>("kevinZombie");
            }
            m_State = State.Wandering;
            RotationAngle = (float)GameObject.RANDOM_GENERATOR.NextDouble();
            Direction.X = (float)Math.Cos(RotationAngle);
            Direction.Y = (float)Math.Sin(RotationAngle);

            Width = GLOBAL_TEXTURE != null ? GLOBAL_TEXTURE.Width : 0;
            Height = GLOBAL_TEXTURE != null ? GLOBAL_TEXTURE.Height : 0;

            if (GLOBAL_TEXTURE != null)
            {
                Bounds.Width = Width;
                Bounds.Height = Height;
                Bounds.X = (int)Position.X - Width / 2;
                Bounds.Y = (int)Position.Y - Height / 2;
                Origin.X = Width / 2;
                Origin.Y = Height / 2;
            }
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

            //get a normalized direction toward the point that was passed in, probably the player
            Vector2 vec = new Vector2(loc.X - Position.X, loc.Y - Position.Y);
            if (vec.LengthSquared() <= (275.0f*275.0f))
            {
                m_State = State.Locked;
            }

            switch (m_State)
            {
                case State.Wandering:
                    if (RANDOM_GENERATOR.Next(150) % 150 == 1)
                    {
                        RotationAngle = (float)RANDOM_GENERATOR.NextDouble() * MathHelper.Pi * 2;
                        Direction.X = (float)Math.Cos(RotationAngle);
                        Direction.Y = (float)Math.Sin(RotationAngle);
                    }
                    break;

                case State.Locked:
                    Direction = vec;
                    RotationAngle = (float)Math.Atan2(vec.Y, vec.X);
                    m_State = State.Locked;
                    SPEED = 2.0f;
                    break;
            }

            Direction = Vector2.Normalize(Direction);
            Vector2 amount = Direction * SPEED;
            base.Move(amount);

            //Later on, remove the clamp to the edge and despawn when too far out of the screen.
            //Position.X = MathHelper.Clamp(Position.X, Width + UI.OFFSET, Game1.GameWidth - (Width / 2));
            //Position.Y = MathHelper.Clamp(Position.Y, Height, Game1.GameHeight - (Height / 2));
            if (!float.IsNaN(this.Position.X) && !float.IsNaN(this.Position.Y))
            {
                _circleBody.Position = ConvertUnits.ToSimUnits(this.Position);
            }

            Bounds.X = (int)Position.X - Width / 2;
            Bounds.Y = (int)Position.Y - Height / 2;
        }
        public void Update(Vector2 playerPosition)
        {

            Move(playerPosition);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GLOBAL_TEXTURE, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, RotationAngle, Origin, 1.0f, SpriteEffects.None, 0f);
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
    }
}
