using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameName1
{
    static class Input
    {
        private static bool isHolding = false;
        public static List<GestureSample> Gestures;
        
        static Input()
        {
            Gestures = new List<GestureSample>();
        }

        public static void ProcessTouchInput(out Vector2 position) 
        {
            position.X = -1;
            position.Y = -1;
            TouchCollection col = TouchPanel.GetState();

            foreach (TouchLocation loc in col)
            {
                if (loc.State == TouchLocationState.Pressed)
                {
                    position = loc.Position;
                }
            }
        }
    }
}