using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Microsoft.Devices.Sensors;

namespace GameName1
{

    static class Input
    {
        public static bool UseAccelerometer = true;
        public static Accelerometer accelerometer;
        public static float AccelerometerAlpha = 0.45f;
        public static Vector3 CurrentAccelerometerValues { get; set; }
        static Input()
        {
            //Gestures = new List<GestureSample>();
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(10);
                accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
            }
            else
            {
                UseAccelerometer = false;
            }
        }
        private static void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            XNAHelper.FauxVector3 temp = new XNAHelper.FauxVector3();
            temp = XNAHelper.XNAHelper.ExtractAccelerometer(e.SensorReading);
            Vector3 tempVector3 = new Vector3();
            tempVector3.X = temp.X * AccelerometerAlpha + temp.X * (1.0f-AccelerometerAlpha);
            tempVector3.Y = temp.Y * AccelerometerAlpha + temp.Y * (1.0f-AccelerometerAlpha);
            tempVector3.Z = temp.Z * AccelerometerAlpha + temp.Z * (1.0f-AccelerometerAlpha);
            CurrentAccelerometerValues = tempVector3;
        }
        public static TouchLocationState CurrentState
        {
            get;
            set;
        }
        public static List<int> TouchIDs = new List<int>();
        public static void ProcessTouchInput(out List<Vector2> touches) 
        {
            touches = new List<Vector2>();
            TouchCollection col = TouchPanel.GetState();
            foreach (TouchLocation loc in col)
            {
                if (loc.State == TouchLocationState.Pressed || loc.State == TouchLocationState.Moved)
                {
                    Vector2 touch = new Vector2(loc.Position.X, loc.Position.Y);
                    touches.Add(touch);
                    CurrentState = loc.State;
                    TouchIDs.Add(loc.Id);
                }
                if (loc.State == TouchLocationState.Released)
                {
                    TouchIDs.Remove(loc.Id);
                }
            }
        }
    }
}