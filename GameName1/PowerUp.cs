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
        public override void LoadContent()
        {
            Texture = TextureBank.GetTexture("Powerup");
            base.LoadContent();
        }
        public override void Load(FarseerPhysics.Dynamics.World world)
        {
            Texture = TextureBank.GetTexture("Powerup");
        }
        public PowerUp() { }
    }
}
