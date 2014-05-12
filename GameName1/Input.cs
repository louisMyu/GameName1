using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Microsoft.Xna.Framework;
using Windows.Devices.Sensors;

namespace GameName1
{

    public class Input
    {
        public static bool UseAccelerometer = true;
        public static Accelerometer accelerometer;
        public static float AccelerometerAlpha = 0.45f;
        public static Vector3 CurrentAccelerometerValues { get; set; }
        public static float Tilt_Threshold = 0.0036f;
        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();
        static Input()
        {
            //Gestures = new List<GestureSample>();
            accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault();
                accelerometer.ReadingChanged += ReadingChanged;
                accelerometer.ReportInterval = 10;
            }
            else
            {
                UseAccelerometer = false;
            }
        }

        private static void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            AccelerometerReading reading = e.Reading;
            Vector3 newAcceration = new Vector3();
            newAcceration.X = (float)(reading.AccelerationX * AccelerometerAlpha + reading.AccelerationX * (1.0f - AccelerometerAlpha)) + 0.25f;
            newAcceration.Y = (float)(reading.AccelerationY * AccelerometerAlpha + reading.AccelerationY * (1.0f - AccelerometerAlpha));
            newAcceration.Z = (float)(reading.AccelerationZ * AccelerometerAlpha + reading.AccelerationZ * (1.0f - AccelerometerAlpha));
            CurrentAccelerometerValues = newAcceration;
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

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }
    }
}