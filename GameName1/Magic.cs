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
    public abstract class Cheat
    {
        protected int m_Duration = 0;
        public abstract void StartEffect(Player p);
        public abstract void EndEffect(Player p);
        //if a cheat effect has some update to do every frame
        //on single use the update will be empty
        public abstract void Update(Player p);
        public bool IsDone()
        {
            return m_Duration == 0;
        }
    }
    //instant effect that occurs when player hits the powerup
    public interface IInstant
    {
        void GetInstantEffect();
    }
    [DataContract]
    public class WrathEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
            //for now hitting the powerup will reset the game
            foreach (GameObject g in ObjectManager.AllGameObjects)
            {
                if (g is IEnemy)
                {
                    ObjectManager.RemoveObject(g);
                }
            }
            ObjectManager.itemMade = false;
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class HealthEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
            ObjectManager.m_Player.LifeTotal = ObjectManager.m_Player.MaxLife;
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class HeartEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
            ObjectManager.m_Player.LifeTotal += 10;
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class BuzzSawEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
            
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class SpeedEffect : Cheat
    {
        private static int m_SpeedBoost = 25;
        public override void StartEffect(Player p)
        {
            Player.VELOCITY += m_SpeedBoost;
            m_Duration = 1000;
        }
        public override void EndEffect(Player p)
        {
            Player.VELOCITY -= m_SpeedBoost;
        }
        public override void Update(Player p)
        {
            --m_Duration;
        }
    }

    public class RandomEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class AddDamageEffect : Cheat
    {
        public override void StartEffect(Player p)
        {
        }
        public override void EndEffect(Player p)
        {
        }
        public override void Update(Player p)
        {
        }
    }

    public class AddTimeEffect : Cheat, IInstant
    {
        public override void StartEffect(Player p)
        {
            Game1.TimeToDeath += TimeSpan.FromSeconds(60);
        }
        public override void EndEffect(Player p)
        {
        }
        public void AddTime(int seconds)
        {
            Game1.TimeToDeath += TimeSpan.FromSeconds(seconds);
        }
        public void GetInstantEffect()
        {
            AddTime(60);
        }
        public override void Update(Player p)
        {
        }
    }
}