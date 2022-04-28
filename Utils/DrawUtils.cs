using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeroDayToolKit.Utils
{
    public class DrawUtils
    {
        public static void drawLine(SpriteBatch spriteBatch, Vector2 origin, Vector2 dest, Color c)
        {
            float y = Vector2.Distance(origin, dest);
            float rotation = (float)Math.Atan2(dest.Y - (double)origin.Y, dest.X - (double)origin.X) + 4.712389f;
            spriteBatch.Draw(Hacknet.Utils.white, origin, new Rectangle?(), c, rotation, Vector2.Zero, new Vector2(1f, y), SpriteEffects.None, 0.5f);
        }

        public static void ClipLineSegmentsInRect(Rectangle bound, Vector2 left, Vector2 right, out Vector2 a, out Vector2 b)
        {
            a = left; b = right;
            bool aModified = false, bModified = false;
            if (a.X < bound.X)
            {
                aModified = true;
                a = new Vector2(bound.X, MathUtils.Ratio(left.X, left.Y, right.X, right.Y, bound.X));
            }
            if (a.X > (bound.X + bound.Width))
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                aModified = true;
                a = new Vector2(bound.X + bound.Width, MathUtils.Ratio(left.X, left.Y, right.X, right.Y, bound.X + bound.Width));
            }
            if (a.Y < bound.Y)
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                aModified = true;
                a = new Vector2(MathUtils.Ratio(left.Y, left.X, right.Y, right.X, bound.Y), bound.Y);
            }
            if (a.Y > (bound.Y + bound.Height))
            {
                if (aModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                a = new Vector2(MathUtils.Ratio(left.Y, left.X, right.Y, right.X, bound.Y + bound.Height), bound.Y + bound.Height);
            }
            if (b.X < bound.X)
            {
                bModified = true;
                b = new Vector2(bound.X, MathUtils.Ratio(left.X, left.Y, right.X, right.Y, bound.X));
            }
            if (b.X > (bound.X + bound.Width))
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                bModified = true;
                b = new Vector2(bound.X + bound.Width, MathUtils.Ratio(left.X, left.Y, right.X, right.Y, bound.X + bound.Width));
            }
            if (b.Y < bound.Y)
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                bModified = true;
                b = new Vector2(MathUtils.Ratio(left.Y, left.X, right.Y, right.X, bound.Y), bound.Y);
            }
            if (b.Y > (bound.Y + bound.Height))
            {
                if (bModified)
                {
                    a = Vector2.Zero;
                    b = Vector2.Zero;
                    return;
                }
                b = new Vector2(MathUtils.Ratio(left.Y, left.X, right.Y, right.X, bound.Y + bound.Height), bound.Y + bound.Height);
            }
        }
    }
}
