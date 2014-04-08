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
            if (powerUp is CheatPowerUp)
            {
                return new WrathEffect();
            }
            return new WrathEffect();
        }
        public virtual void GetEffect()
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

        public override void GetEffect()
        {
            //for now hitting the powerup will reset the game
            foreach (GameObject g in ObjectManager.AllGameObjects)
            {
                if (g is Zombie)
                {
                    ObjectManager.RemoveObject(g);
                }
            }
            ObjectManager.itemMade = false;
            return;
        }

        public override Texture2D GetTexture()
        {
            return WrathTexture;
        }
    }
}
