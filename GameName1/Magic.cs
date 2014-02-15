using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    [KnownType(typeof(WrathEffect))]
    [DataContract]
    public class Magic
    {
        [IgnoreDataMember]
        protected static Texture2D WrathTexture;  //have all the textures static, and in the base class, one instance
        [IgnoreDataMember]
        private static bool TexturesLoaded = false;

        [DataMember]
        public string Name { get; set; }
        
        //called once
        public static void TextureInit(ContentManager content)
        {
            if (!TexturesLoaded)
            {
                if (WrathTexture == null)
                    WrathTexture = content.Load<Texture2D>("Powerup");
                TexturesLoaded = true;
            }
        }

        public static Magic GetMagicType(GameObject powerUp)
        {
            if (powerUp is PowerUp)
            {
                return new WrathEffect();
            }
            return new WrathEffect();
        }
        public virtual void GetEffect(List<GameObject>exists, List<Zombie> zombies)
        {

        }

        public virtual Texture2D GetTexture()
        {
            return WrathTexture;
        }

        public Magic()
        {
        }
    }

    [DataContract]
    public class WrathEffect : Magic
    {
        public WrathEffect()
        {
            
        }

        public override void GetEffect(List<GameObject> exists, List<Zombie> zombies)
        {
            //for now hitting the powerup will reset the game
            List<GameObject> removedAtEnd = new List<GameObject>();
            foreach (GameObject g in exists)
            {
                if (g is Zombie)
                {
                    ((Zombie)g).CleanBody();
                    zombies.Remove((Zombie)g);
                    removedAtEnd.Add(g);
                }
            }
            Game1.ZombiesSpawned = false;
            Game1.GameTimer = 0;
            Game1.itemMade = false;
            foreach (GameObject g in removedAtEnd)
            {
                exists.Remove(g);
            }
            return;
        }

        public override Texture2D GetTexture()
        {
            return WrathTexture;
        }
    }
}
