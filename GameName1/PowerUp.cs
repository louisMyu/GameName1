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

    public class CheatPowerUp : PowerUp
    {
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Load(FarseerPhysics.Dynamics.World world)
        {
            base.Load(world);
        }
        public CheatPowerUp() { }
    }
    public class WeaponPowerUp : PowerUp
    {
        public enum WeaponType
        {
            Shotgun,
            Rifle,
            Plasma
        }
        private WeaponType Type;
        public static Weapon GetWeaponType(WeaponPowerUp p)
        {
            switch (p.Type)
            {
                case WeaponType.Shotgun :
                    return new Shotgun();
                case WeaponType.Rifle:
                    return new Rifle();
                case WeaponType.Plasma:
                    return new Plasma();
                default:
                    return new Shotgun();
            }
            
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Load(FarseerPhysics.Dynamics.World world)
        {
            base.Load(world);
        }
        public WeaponPowerUp(WeaponType type)
        {
            Type = type;
        }
    }
}
