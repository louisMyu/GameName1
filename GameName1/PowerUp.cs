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
        public enum CheatTypes
        {
            Wrath,
            Health
        }
        public CheatTypes CheatType;
        public Magic CheatEffect;
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Load(FarseerPhysics.Dynamics.World world)
        {
            base.Load(world);
        }
        public CheatPowerUp() { }
        public CheatPowerUp(CheatTypes type)
        {
            CheatType = type;
            CreateCheat();
        }
        private void CreateCheat()
        {
            string temp;
            switch (CheatType)
            {
                case CheatTypes.Wrath:
                    temp = "Powerup";
                    CheatEffect = new WrathEffect();
                    break;
                case CheatTypes.Health:
                    temp = "MedPack";
                    CheatEffect = new HealthEffect();
                    break;
                default:
                    temp = "Powerup";
                    CheatEffect = new WrathEffect();
                    break;
            }
            Texture = TextureBank.GetTexture(temp);
        }
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
            SetWeaponPowerUpTexture(Type);
        }
        public override void Load(FarseerPhysics.Dynamics.World world)
        {
            base.Load(world);
        }
        public WeaponPowerUp(WeaponType type)
        {
            Type = type;
            
        }
        private void SetWeaponPowerUpTexture(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Shotgun:
                    break;
                case WeaponType.Rifle:
                    break;
                case WeaponType.Plasma:
                    Texture = TextureBank.GetTexture("PlasmaIcon");
                    break;
                default :
                    Texture = TextureBank.GetTexture("PlasmaIcon");
                    break;
            }
        }
    }
}
