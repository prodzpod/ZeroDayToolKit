using System;
using Microsoft.Xna.Framework;
using Hacknet;
using Hacknet.Gui;

namespace ZeroDayToolKit.Executibles
{
    public class SSHSwiftEXE : ZeroDayEXE
    {
        public SSHSwiftEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(22, 4f, 200, ">S>.SSH.$w!f7.>S>", ">S> Let's do this. >S>", ">S> SSH? Swiftly Shafted. Thank me later. >S>");
        }

        public override void incrementLife(float t)
        {
            life += t * (float)ZeroDayToolKit.rnd.NextDouble() * 2;
        }

        private const int WIDTH = 8;
        private const int HEIGHT = 8;
        public override void Draw(float t)
        {
            TextItem.doFontLabel(new Vector2(bounds.X + 2, bounds.Y + 14), done ? "And done." : "Sit back and watch it go.", GuiData.UITinyfont, new Color?(Hacknet.Utils.AddativeWhite * 0.8f * fade), bounds.Width - 6);
            Rectangle source = new Rectangle(0, 0, Bounds.Width / WIDTH - 4, Bounds.Height / HEIGHT - 8);
            for (int x = 0; x < WIDTH; x++)
            {
                source.X = Bounds.X + 4 + (x * (Bounds.Width - 4) / WIDTH);
                for (int y = 0; y < HEIGHT; y++) // grid
                {
                    source.Y = Bounds.Y + 32 + (y * (Bounds.Height - 30) / HEIGHT);
                    Rectangle destination = source;
                    float progress = Math.Max(0f, 1f - ((1f - ((float)(x + y) / (WIDTH + HEIGHT))) * 0.5f + (life / runTime)));
                    if (ZeroDayToolKit.rnd.NextDouble() < progress) progress = (float)ZeroDayToolKit.rnd.NextDouble();
                    Color color = Color.Lerp(os.unlockedColor, os.brightLockedColor, progress);
                    int num = (int)(progress * 99);
                    float offsetx = (Bounds.Width * 0.5f / WIDTH) - ((0.5f + num.ToString().Length) * 3.75f);
                    float offsety = (Bounds.Height * 0.5f / HEIGHT) - 12f;
                    spriteBatch.Draw(Hacknet.Utils.white, destination, color * fade);
                    spriteBatch.DrawString(GuiData.UITinyfont, num.ToString(), new Vector2(source.X + offsetx, source.Y + offsety), Color.White * fade);
                }
            }
            base.Draw(t);
        }
    }
}
