using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Microsoft.Devices.Sensors;

namespace FarSeerTest
{
    static class Input
    {
        //public static List<GestureSample> Gestures;
        public static Accelerometer accelerometer;

        static Input()
        {
            //Gestures = new List<GestureSample>();
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
            }
        }

        public static TouchLocationState CurrentState
        {
            get;
            set;
        }
        public static void ProcessTouchInput(out List<Vector2> touches) 
        {
            touches = new List<Vector2>();
            TouchCollection col = TouchPanel.GetState();
            foreach (TouchLocation loc in col)
            {
                if (loc.State == TouchLocationState.Pressed)
                {
                    Vector2 touch = new Vector2(loc.Position.X, loc.Position.Y);
                    touches.Add(touch);
                    CurrentState = loc.State;
                }
            }
        }
    }
}