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
        public static float Tilt_Threshold = 0.0036f;
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
            tempVector3.X = (temp.X * AccelerometerAlpha + temp.X * (1.0f-AccelerometerAlpha)) + 0.25f;
            tempVector3.Y = temp.Y * AccelerometerAlpha + temp.Y * (1.0f-AccelerometerAlpha);
            tempVector3.Z = temp.Z * AccelerometerAlpha + temp.Z * (1.0f-AccelerometerAlpha);
            CurrentAccelerometerValues = tempVector3;
        }
        public static TouchLocationState CurrentState
        {
            get;
            set;
        }
        public static List<TouchLocation> TouchesCollected = new List<TouchLocation>();
        public static void ProcessTouchInput() 
        {
            lock (TouchesCollected)
            {
                TouchesCollected.Clear();
                TouchCollection col = TouchPanel.GetState();
                foreach (TouchLocation touch in col)
                {
                    TouchesCollected.Add(touch);
                }
            }
        }
    }
}