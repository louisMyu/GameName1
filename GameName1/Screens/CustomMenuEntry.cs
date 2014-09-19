
using Microsoft.Xna.Framework;

namespace GameName1
{
    class CustomMenuEntry : MenuEntry
    {
        private Rectangle Bounds;

        public CustomMenuEntry()
            : base("")
        {
        }
        public CustomMenuEntry(Rectangle bounds)
            : base("")
        {
            Bounds = bounds;
        }
        public override int GetHeight()
        {
            return Bounds.Width;
        }
        public override int GetWidth()
        {
            return Bounds.Height;
        }
    }
}
