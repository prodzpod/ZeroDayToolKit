using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeroDayToolKit.Utils
{
    public class DrawUtils
    {
        public static Vector2 correctBounds(Rectangle bound, Vector2 point)
        {
            return point * bound.Width / 2 + new Vector2(bound.Width / 2 + bound.X, bound.Height / 2 + bound.Y);
        }
        public static Rectangle correctBounds(Rectangle bound, Vector4 point)
        {
            int vs = Math.Min(bound.Width, bound.Height);
            return new Rectangle(bound.X + ((bound.Width - vs) / 2) + (int)(vs * (point.X / 2 + 0.5f)), bound.Y + ((bound.Height - vs) / 2) + (int)(vs * (point.Y / 2 + 0.5f)), (int)(vs * (point.Z / 2)), (int)(vs * (point.W / 2)));
        }
        public static Vector2 toPolar(float r, float theta)
        {
            return new Vector2(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta));
        }
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

        public static void ColorToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R, g = color.G, b = color.B;
            float max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b)), d = max - min;
            h = 0;
            s = (max == 0 ? 0 : d / max);
            v = max / 255;
            if (max == min) h = 0;
            else if (max == r) h = (g - b) + d * (g < b ? 6 : 0);
            else if (max == g) h = (b - r) + d * 2;
            else if (max == b) h = (r - g) + d * 4;
            h /= 6 * d;
        }

        public static Color ColorFromHSV(float h, float s, float v)
        {
            float r = 0, g = 0, b = 0, 
                i = (float)Math.Floor(h * 6),
                f = h * 6 - i,
                p = v * (1 - s),
                q = v * (1 - f * s),
                t = v * (1 - (1 - f) * s);
            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }
            return new Color(r, g, b);
        }
    }
}
