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

    //0_o
    public class WidgetTree
    {
        private List<WidgetTree> Children;
        private List<Rectangle> HitableObjects;
        private Dictionary<Rectangle, ColorTexture> DrawableAreas;
        private Dictionary<Rectangle, ColorString> DrawableTexts;
        private Rectangle BaseContainer;
        public WidgetTree(Rectangle baseArea)
        {
            Children = null;
            HitableObjects = new List<Rectangle>();
            DrawableAreas = new Dictionary<Rectangle, ColorTexture>();
            DrawableTexts = new Dictionary<Rectangle, ColorString>();
            BaseContainer = baseArea;
        }
        public void AddHitArea(Rectangle rec)
        {
            HitableObjects.Add(rec);
        }
        public void AddDrawArea(Rectangle area, ColorTexture tex)
        {
            DrawableAreas.Add(area, tex);
        }
        public void AddTextArea(Rectangle area, string text)
        {
            
        }
        public void UpdatePositions(Vector2 delta)
        {
            BaseContainer.X += (int)delta.X;
            BaseContainer.Y += (int)delta.Y;
            for (int x = 0; x < HitableObjects.Count; ++x)
            {
                Rectangle rec = HitableObjects[x];
                rec.X += (int)delta.X;
                rec.Y += (int)delta.Y;
                HitableObjects[x] = rec;
            }
            if (Children != null)
            {
                foreach (WidgetTree child in Children)
                {
                    child.UpdatePositions(delta);
                }
            }
        }
        public void AddWidgetTree(WidgetTree widgetTree)
        {
            if (Children == null)
            {
                Children = new List<WidgetTree>();
            }
            Children.Add(widgetTree);
        }
        //returns an empty rectangle on false
        public Rectangle CheckCollision(Point p)
        {
            if (Children != null)
            {
                foreach (WidgetTree tree in Children)
                {
                    Rectangle temp;
                    temp = tree.CheckCollision(p);
                    if (temp.Width > 0)
                    {
                        return temp;
                    }
                }
            }
            foreach (Rectangle rec in HitableObjects)
            {
                Rectangle trueRec = new Rectangle(rec.X - (rec.Width/2) + BaseContainer.X, rec.Y - (rec.Height/2) + BaseContainer.Y, rec.Width, rec.Height);
                if (trueRec.Contains(p))
                {
                    return rec;
                }
            }
            return new Rectangle();
        }
        //this color parameter needs to be removed in the future
        public void StartDrawWidgets(SpriteBatch _spriteBatch, Rectangle where)
        {
            Queue<WidgetTree> queue = new Queue<WidgetTree>();
            queue.Enqueue(this);
            while (queue.Count > 0) {
                WidgetTree child = queue.Dequeue();
                child.DrawWidgets(_spriteBatch, where);
                if (child.Children != null) 
                {
                    foreach (WidgetTree widgetTree in child.Children)
                    {
                        queue.Enqueue(widgetTree);
                    }
                }
            }
        }
        //TODO: Remove this color parameter
        private void DrawWidgets(SpriteBatch _spriteBatch, Rectangle where)
        {
            Rectangle temp = new Rectangle();
            foreach (KeyValuePair<Rectangle, ColorTexture> entry in DrawableAreas)
            {
                temp = entry.Key;
                temp.X += (BaseContainer.X + where.X);
                temp.Y += (BaseContainer.Y + where.Y);
                _spriteBatch.Draw(entry.Value.Texture, temp, null, entry.Value.Color, Utilities.DegreesToRadians(90f), new Vector2((entry.Value.Texture.Width / 2), (entry.Value.Texture.Height / 2)), 
                                    SpriteEffects.None, 0);
            }
            foreach (KeyValuePair<Rectangle, ColorString> entry in DrawableTexts)
            {
                Vector2 measuredString = entry.Value.Font.MeasureString(entry.Value.Text);
                Vector2 stringOrigin = new Vector2(measuredString.X / 2, measuredString.Y / 2);
                temp = entry.Key;
                temp.X += (BaseContainer.X + where.X);
                temp.Y += (BaseContainer.Y + where.Y);
                _spriteBatch.DrawString(entry.Value.Font, entry.Value.Text, new Vector2(temp.X, temp.Y), entry.Value.Color, Utilities.DegreesToRadians(90f), stringOrigin, new Vector2(0,0),
                                    SpriteEffects.None, 0);
            }
        }
    }
    public class ColorString
    {
        public SpriteFont Font;
        public string Text;
        public Color Color;
        public ColorString(SpriteFont f, string t, Color c)
        {
            Font = f;
            Text = t;
            Color = c;
        }
    }
    public class ColorTexture
    {
        public Texture2D Texture;
        public Color Color;
        public ColorTexture(Texture2D tex, Color c)
        {
            Texture = tex;
            Color = c;
        }
    }
}
