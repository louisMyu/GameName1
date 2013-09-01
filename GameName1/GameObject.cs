using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class GameObject
    {
        protected static Random RANDOM_GENERATOR = new Random(69);
        public Vector2 Position;
        public Texture2D Texture;
        public int Width;
        public int Height;
        public Rectangle Bounds;
        public Vector2 Origin = new Vector2();
        public Vector2 Direction = new Vector2();
        private float m_RotationAngle = 0.0f;

        public float RotationAngle
        {
            get
            {
                return m_RotationAngle;
            }
            set
            {
                m_RotationAngle = value % (MathHelper.Pi * 2);
            }
                
        }
        public GameObject()
        {
            if (Bounds == null)
            {
                Bounds = new Rectangle();
            }
        }
        public virtual void Init(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public virtual void Update()
        {
        }
        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content) 
        {
            Width = Texture != null ? Texture.Width : 0;
            Height = Texture != null ? Texture.Height : 0;
            
            if (Texture != null)
            {
                Bounds.Width = Width;
                Bounds.Height = Height;
                Bounds.X = (int)Position.X - Width / 2;
                Bounds.Y = (int)Position.Y - Height / 2;
                Origin.X = Width / 2;
                Origin.Y = Height / 2;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, RotationAngle, Origin, 1.0f, SpriteEffects.None, 0f);
        }

        public virtual void Move(Vector2 amount)
        {
            Position += amount;
        }
    }
}
