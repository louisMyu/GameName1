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
    public class Magic
    {
        Texture2D texture;
        protected Magic(Texture2D tex)
        {
            texture = tex;
        }
    }
    [DataContract]
    public class WrathEffect : Magic, IMagic
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

    public class HealthEffect : Magic, IMagic
    {
        public void GetEffect()
        {
            ObjectManager.m_Player.LifeTotal = ObjectManager.m_Player.MaxLife;
        }
    }
}
