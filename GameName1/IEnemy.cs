using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    public interface IEnemy
    {
        int GetHealth();
        void AddToHealth(int amount);
        void ApplyLinearForce(Vector2 angle, float amount);
        void CleanBody();
        int GetDamageAmount();
    }
}
