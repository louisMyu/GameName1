using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Zombie : GameObject
    {
        private static Texture2D GLOBAL_TEXTURE = null;
        private float SPEED = 0.5f;
        private enum State
        {
            Wandering,
            Locked,
            Dead
        }
        private State m_State;
        public Zombie() : base()
        {
        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            if (GLOBAL_TEXTURE == null)
            {
                GLOBAL_TEXTURE = Content.Load<Texture2D>("ZombieFace");
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
        }

        //moves a set amount per frame toward a certain location
        public override void Move(Microsoft.Xna.Framework.Vector2 loc)
        {
            //get a normalized direction toward the point that was passed in, probably the player
            Vector2 vec = new Vector2(loc.X - Position.X, loc.Y - Position.Y);
            if (vec.LengthSquared() <= (150.0f*150.0f))
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
            Position.X = MathHelper.Clamp(Position.X, Width, Game1.GameWidth - (Width / 2));
            Position.Y = MathHelper.Clamp(Position.Y, Height, Game1.GameHeight - (Height / 2));
            Bounds.X = (int)Position.X - Width / 2;
            Bounds.Y = (int)Position.Y - Height / 2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GLOBAL_TEXTURE, Position, null, Color.White, RotationAngle, Origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
