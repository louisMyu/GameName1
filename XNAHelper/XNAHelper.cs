using Microsoft.Devices.Sensors;

namespace XNAHelper
{
    public struct FauxVector3
    {
        public float X;
        public float Y;
        public float Z;

        public FauxVector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    public class XNAHelper
    {
        public static FauxVector3 ExtractGravity(MotionReading reading)
        {
            return new FauxVector3(reading.Gravity.X, reading.Gravity.Y, reading.Gravity.Z);
        }
        public static FauxVector3 ExtractAccelerometer(AccelerometerReading reading)
        {
            return new FauxVector3(reading.Acceleration.X, reading.Acceleration.Y, reading.Acceleration.Z);
        }
    }
}