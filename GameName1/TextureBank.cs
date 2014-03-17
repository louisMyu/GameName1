using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace GameName1
{
    public static class TextureBank
    {
        public static Dictionary<string, Texture2D> Bank = new Dictionary<string, Texture2D>();
        //public static void LoadTexture(string name, ContentManager content)
        //{
        //    if (Bank.ContainsKey(name))
        //    {
        //        return;
        //    }
        //    Bank[name] = content.Load<Texture2D>(name);
        //}
        public static Texture2D GetTexture(string name, ContentManager content)
        {
            if (!Bank.ContainsKey(name))
            {
                Bank[name] = content.Load<Texture2D>(name);
            }
            return Bank[name];
        }
    }
}
