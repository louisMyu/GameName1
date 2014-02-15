using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    [KnownType(typeof(PowerUp))]
    [KnownType(typeof(GameObject))]
    [DataContract]
    public class PowerUp : GameObject
    {
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Texture = TextureBank.GetTexture("Powerup", content);
            base.LoadContent(content);
        }
        public override void Load(Microsoft.Xna.Framework.Content.ContentManager content, FarseerPhysics.Dynamics.World world)
        {
            Texture = TextureBank.GetTexture("Powerup", content);
        }
        public PowerUp() { }
    }
}
