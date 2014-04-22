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
    public interface IMagic
    {
        void GetEffect();
    }
    [DataContract]
    public class WrathEffect : IMagic
    {
        public void GetEffect()
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
    }

    public class HealthEffect : IMagic
    {
        public void GetEffect()
        {
            ObjectManager.m_Player.LifeTotal = ObjectManager.m_Player.MaxLife;
        }
    }

    public class HeartEffect : IMagic
    {
        public void GetEffect()
        {
            ObjectManager.m_Player.LifeTotal += 10;
        }
    }

    public class BuzzSawEffect : IMagic
    {
        public void GetEffect()
        {
            
        }
    }

    public class SpeedEffect : IMagic
    {
        public void GetEffect()
        {
        }
    }

    public class RandomEffect : IMagic
    {
        public void GetEffect()
        {
        }
    }

    public class DamageEffect : IMagic
    {
        public void GetEffect()
        {
        }
    }
}
