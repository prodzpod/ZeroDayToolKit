using Microsoft.Xna.Framework;
using Hacknet;

namespace ZeroDayToolKit.Executibles
{
    public class PacketHeaderInjectionEXE : ZeroDayEXE
    {
        public PacketHeaderInjectionEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(80, 30f, 50, ":pkthdr::", "[pkthdr.exe] onLoad()::", "[pkthdr.exe] onComplete()::");
        }

        private const float PERIOD = 5f;
        public override void Draw(float t)
        {
            float offset = (life % PERIOD) / PERIOD;
            bool flip = life % (PERIOD * 2) < PERIOD;
            float temp = (1f - (life / runTime));
            int bound = (int)(120f * (1f - (temp * temp))) + 1;
            float unitWidth = Bounds.Width / bound;
            float unitHeight = Bounds.Height - 14;
            Vector2 pre = new Vector2(Bounds.X, Bounds.Y + 14f + (flip ? (offset * unitHeight) : ((1 - offset) * unitHeight)));
            Vector2 post;
            for (int i = 0; i < bound; i++)
            {
                post = new Vector2(Bounds.X + ((i + 1) * unitWidth), Bounds.Y + 14f + (unitHeight * (float)ZeroDayToolKit.rnd.NextDouble()));
                drawLine(pre, post, os.defaultHighlightColor * fade * 0.5f);
                pre = post;
            }
            pre = new Vector2(Bounds.X, Bounds.Y + 14f + (flip ? (offset * unitHeight) : ((1 - offset) * unitHeight)));
            post = new Vector2(Bounds.X + offset * unitWidth, Bounds.Y + 14f + (flip ? unitHeight : 0));
            for (int i = 0; i < bound; i++)
            {
                drawLine(pre, post, Color.White * fade);
                pre = post;
                post = new Vector2(Bounds.X + (offset + i + 1f) * unitWidth, Bounds.Y + 14f + ((flip ^ (i % 2 == 0)) ? 0f : unitHeight));
            }
            drawLine(pre, new Vector2(Bounds.X + Bounds.Width, Bounds.Y + 14f + (flip ? ((1f - offset) * unitHeight) : (offset * unitHeight))), Color.White * fade);
            base.Draw(t);
        }
    }
}
