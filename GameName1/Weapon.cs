using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Weapon : GameObject
    { 

        public Weapon() 
        {
            int x = 2;
        }
        public new void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.LoadContent(content);
        }

        //this should be called every update if it exists for the player
        public virtual void Update()
        {
        }

        public virtual void CheckCollision(GameObject ob)
        {

        }
    }
}
