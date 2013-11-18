using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    class Utilities
    {
        public static float DegreesToRadians(float degrees)
        {
            return degrees *  (float)Math.PI / 180.0f ;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (180.0f) / (float)Math.PI;
        }

        public static bool PointIntersectsRectangle(Vector2 point, Rectangle rec)
        {
            return (point.X >= rec.Left && point.Y >= rec.Top && point.X <= rec.Right && point.Y <= rec.Bottom);
        }

        

    }
}
