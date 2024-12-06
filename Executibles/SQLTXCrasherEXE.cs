using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Hacknet;
using Hacknet.Gui;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Executibles
{
    public class SQLTXCrasherEXE : ZeroDayEXE
    {
        public SQLTXCrasherEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(1433, 10f, 10f, 240, "THE_LAST_RESORT", "" +
                "================================\n" +
                "//                                             \\\\\n" +
                "\\\\      SQL_TX_Crasher v0.0.5       //\n" +
                "//       a.k.a. THE LAST RESORT       \\\\\n" +
                "\\\\                                             //\n" +
                "================================", "" +
                "================================\n" +
                "//                                            \\\\\n" +
                "\\\\           PROCESS                      //\n" +
                "//                FINISHED                \\\\\n" +
                "\\\\                                            //\n" +
                "================================");
        }
        public override void Update(float t)
        {
            if (!done && life >= runTime)
            {
                done = true;
                Computer c = Programs.getComputer(os, targetIP);
                c.openPort(port, os.thisComputer.ip);
                foreach (string line in endMessage.Split('\n')) os.write(line);
                if (c.hasProxy)
                {
                    c.proxyOverloadTicks = c.startingOverloadTicks;
                    c.proxyActive = true;
                    os.write("THREAT DETECTED: Proxy Reenabled");
                }
                if (c.firewall != null)
                {
                    c.firewall.solution = Hacknet.Utils.FlipRandomChars(c.firewall.solution, 0.5);
                    c.firewall.solved = false;
                    c.firewall.resetSolutionProgress();
                    os.write("THREAT DETECTED: Firewall Reenabled");
                }
            }
            base.Update(t);
        }

        public override void Draw(float t)
        {
            int minb = Math.Max(0, Math.Min(Bounds.Width, Bounds.Height - 14) / 2);
            Vector2 center = new(Bounds.X + (Bounds.Width / 2), Bounds.Y + 7 + (Bounds.Height / 2));
            Rectangle bound = new(Bounds.X, Bounds.Y + 14, Bounds.Width, Bounds.Height - 14);
            spriteBatch.Draw(Hacknet.Utils.white, new Rectangle(Bounds.X, Bounds.Y + 7 + (Bounds.Height / 2) - 5, Bounds.Width, 10), (done ? os.unlockedColor : os.lockedColor) * 0.5f * fade);
            for (float q = 0.1f; q <= 1f; q += 0.1f)
            {
                float angle = (float)Math.Sin((q * 4 + life) / 2.5f);
                Color color = Color.Lerp(Color.White, done ? os.brightUnlockedColor : os.brightLockedColor, Math.Abs(angle)) * q * fade;
                Color color2 = Color.Lerp(Color.White, done ? os.unlockedColor : os.lockedColor, Math.Abs(angle)) * q * fade;
                Rectangle dest = new((int)center.X, (int)center.Y, minb, minb);
                spriteBatch.Draw(Hacknet.Utils.white, dest, new Rectangle?(), color, (float)Math.PI * angle, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
                Vector2 top = new(center.X + (minb * (float)Math.Sin(Math.PI * angle)), center.Y - (minb * (float)Math.Cos(Math.PI * angle)));
                Vector2 left = new(center.X - (minb * (float)Math.Cos(Math.PI * angle)), center.Y - (minb * (float)Math.Sin(Math.PI * angle)));
                Vector2 bottom = new(center.X - (minb * (float)Math.Sin(Math.PI * angle)), center.Y + (minb * (float)Math.Cos(Math.PI * angle)));
                Vector2 right = new(center.X + (minb * (float)Math.Cos(Math.PI * angle)), center.Y + (minb * (float)Math.Sin(Math.PI * angle)));
                Vector2 a, b;
                DrawUtils.ClipLineSegmentsInRect(bound, top, left, out a, out b);
                drawLine(a, b, color2);
                DrawUtils.ClipLineSegmentsInRect(bound, left, bottom, out a, out b);
                drawLine(a, b, color2);
                DrawUtils.ClipLineSegmentsInRect(bound, bottom, right, out a, out b);
                drawLine(a, b, color2);
                DrawUtils.ClipLineSegmentsInRect(bound, right, top, out a, out b);
                drawLine(a, b, color2);
            }
            Rectangle midbar = new(Bounds.X, Bounds.Y + 9 + (Bounds.Height / 3), Bounds.Width, Bounds.Height / 3);
            spriteBatch.Draw(Hacknet.Utils.white, midbar, Color.Black * 0.5f * fade);
            if ((!done && (ZeroDayToolKit.rnd.NextDouble() < (life / runTime))) || (done && (ZeroDayToolKit.rnd.NextDouble() > ((life - runTime) / exitTime))))
                TextItem.doCenteredFontLabel(midbar, LocaleTerms.Loc("-  " + (done ? "Completed." : "Crashing...") + "  -"), GuiData.smallfont, Color.White);
            base.Draw(t);
        }
    }
}
