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
    [KnownType(typeof(GameObject))]
    [KnownType(typeof(Zombie))]
    [KnownType(typeof(PowerUp))]
    [DataContract]
    public class GameObject
    {
        protected static Random RANDOM_GENERATOR = new Random(69);
        [IgnoreDataMember]
        private Vector2 m_Position;
        [DataMember]
        public Vector2 Position { get { return m_Position; } set { m_Position = value; } }
        [IgnoreDataMember]
        public Texture2D Texture;
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [IgnoreDataMember]
        public Rectangle m_Bounds;
        [DataMember]
        public Rectangle Bounds { get { return m_Bounds; } set { m_Bounds = value; } }
        [IgnoreDataMember]
        public Vector2 m_Origin = new Vector2();
        [DataMember]
        public Vector2 Origin { get { return m_Origin; } set { m_Origin = value; } }
        [IgnoreDataMember]
        public Vector2 m_Direction = new Vector2();
        [DataMember]
        public Vector2 Direction { get { return m_Direction; } set { m_Direction = value; } }
        [IgnoreDataMember]
        private float m_RotationAngle = 0.0f;
        [DataMember]
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
        public bool CanDelete = false;
        public GameObject()
        {
            if (m_Bounds == null)
            {
                m_Bounds = new Rectangle();
            }
        }
        public virtual void Init(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public virtual void Update(Player player, TimeSpan elapsedTime)
        {
        }
        public virtual void LoadContent() 
        {
            Width = Texture != null ? Texture.Width : 0;
            Height = Texture != null ? Texture.Height : 0;
            
            if (Texture != null)
            {
                m_Bounds.Width = Width;
                m_Bounds.Height = Height;
                m_Bounds.X = (int)Position.X - Width / 2;
                m_Bounds.Y = (int)Position.Y - Height / 2;
                m_Origin.X = Width / 2;
                m_Origin.Y = Height / 2;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, RotationAngle, m_Origin, 1.0f, SpriteEffects.None, 0f);
        }

        public virtual void Move(Vector2 amount, TimeSpan elapsedTime)
        {
            Position += amount * 60 / 1000 * elapsedTime.Milliseconds;
        }
        public virtual void Save()
        {
        }
        public virtual void Load(FarseerPhysics.Dynamics.World world)
        {
        }
        public virtual void CheckCollisions(GameObject obj)
        {

        }
    }
    [DataContract]
    public class GameObjectStore
    {
        [DataMember]
        public List<GameObject> GameObjectList { get { return m_GameObjectList; } set { m_GameObjectList = value; } }
        [IgnoreDataMember]
        private List<GameObject> m_GameObjectList = new List<GameObject>();
        public GameObjectStore() { }

        public void Save()
        {
            Storage.Save<List<GameObject>>("GameObjects", "ObjectList.dat", GameObjectList);
        }
        public void Load()
        {
            GameObjectList = Storage.Load<List<GameObject>>("GameObjects", "ObjectList.dat");
            if (GameObjectList == null)
            {
                GameObjectList = new List<GameObject>();
            }
        }
    }
}
