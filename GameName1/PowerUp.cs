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
    
    //when creating new cheat powerups, add to enum, add to createCheat
    //add to magic for related effect
    public class CheatPowerUp : PowerUp
    {
        public enum CheatTypes
        {
            Wrath,
            Health
        }
        public CheatTypes CheatType;
        public IMagic CheatEffect;
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
    //when creating new weapon types add to Enum, add to getweapontype, add to
    //setting the texture for that weapon, add new class for weapon deriving from weapon
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
            //need to set texture before calling base loadcontent to create collision bounds
            SetWeaponPowerUpTexture(Type);
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
        private void SetWeaponPowerUpTexture(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Shotgun:
                    Texture = TextureBank.GetTexture("ShotgunIcon");
                    break;
                case WeaponType.Rifle:
                    Texture = TextureBank.GetTexture("RifleIcon");
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
