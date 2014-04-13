using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        //The amount of scaling that needs to be applied to a sprite with width and height of baseVec
        //so that it matches the width and height for result
        public static Vector2 GetSpriteScaling(Vector2 result, Vector2 baseVec)
        {
            Vector2 temp = new Vector2();
            temp.X = result.X / baseVec.X;
            temp.Y = result.Y / baseVec.Y;
            return temp;
        }

    }
}
