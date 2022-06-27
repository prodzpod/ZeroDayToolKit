using System;
using Microsoft.Xna.Framework;
using Hacknet;
using Hacknet.Gui;
using Pathfinder.Util;
using Pathfinder.Port;

using ZeroDayToolKit.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace ZeroDayToolKit.Executibles
{
    public class PortBackdoorEXE : ZeroDayEXE
    {
        public PortBackdoorEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(0, 40f, 5f, 400, "thanks from /el/ <3", "TOP TEXT", "BOTTOM TEXT");
        }

        public override void LoadContent()
        {
            Computer c = ComputerLookup.FindByIp(targetIP);
            port = c.GetDisplayPortNumberFromCodePort(originPort);
            if (noProxy && c.proxyActive)
            {
                os.write("Proxy Active");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else if ((Args.Length > 1 && (!int.TryParse(Args[1], out _) || int.Parse(Args[1]) != port)) || (Args.Length <= 1 && port != 0))
            { // if no args given, use 65535
                os.write("Target Port is Closed");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else foreach (string line in startMessage.Split('\n')) os.write(line);
            if (!noHostile) Programs.getComputer(os, targetIP).hostileActionTaken();
        }
        public override void Update(float t)
        {
            if (!done && life >= runTime)
            {
                Computer c = Programs.getComputer(os, targetIP);
                done = true;
                foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                if (!ComUtils.isPortOpen(c, originPort)) // now it has port 0
                    c.AddPort(PortManager.GetPortRecordFromNumber(originPort).CreateState(c, true));
                c.openPort(port, os.thisComputer.ip);
                foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                foreach (string line in endMessage.Split('\n')) os.write(line);
            }
            base.Update(t);
        }

        public static int[] edgeA = { 3, 2, 6, 7, 1, 0, 3, 5, 4, 1, 6, 5, 14, 15, 10, 11, 10, 8, 9, 8, 12, 13, 12, 14, 9, 93, 94, 7, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 8, 93, 7, 107, 114, 117, 105, 98, 102, 107, 97, 99, 1, 3, 103, 213, 105, 13, 109, 118, 223, 112, 224, 111, 220, 116, 222, 96, 98, 100, 102, 104, 106, 107, 226, 216, 215, 221, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 198, 197, 202, 195, 203, 192, 200, 192, 201, 185, 204, 198, 206, 185, 209, 189, 211, 212, 176, 177, 210, 175, 207, 187, 214, 183, 203, 200, 202, 201, 205, 204, 205, 207, 206, 210, 209, 213, 211, 210, 212, 198, 197, 175, 193, 186, 195, 94, 181, 179, 191, 174, 183, 174, 182, 217, 215, 216, 223, 220, 222, 219, 218, 224, 226, 227, 221, 84, 90, 82, 222, 219 };
        public static int[] edgeB = { 2, 6, 7, 3, 0, 2, 1, 4, 0, 5, 4, 7, 15, 10, 11, 14, 8, 9, 11, 12, 13, 9, 15, 13, 1, 11, 5, 14, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 15, 9, 11, 108, 99, 96, 110, 115, 113, 95, 96, 98, 13, 93, 102, 14, 104, 94, 106, 95, 89, 103, 88, 104, 91, 97, 85, 95, 97, 99, 101, 103, 105, 106, 87, 83, 92, 86, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 194, 187, 176, 190, 178, 196, 180, 191, 182, 188, 184, 177, 186, 195, 188, 175, 190, 193, 189, 199, 194, 178, 196, 183, 199, 184, 200, 201, 203, 204, 202, 206, 208, 208, 209, 207, 211, 212, 213, 214, 214, 192, 180, 197, 179, 181, 179, 208, 185, 177, 189, 205, 181, 191, 187, 216, 217, 218, 219, 215, 221, 220, 222, 223, 225, 224, 226, 218, 219, 217, 4, 0 };
        public static float[] vertX = { 0.1057f, 0.0209f, 0.0838f, -0.0065f, 0.0628f, -0.0461f, 0.0409f, -0.0735f, -0.6514f, -0.5616f, -0.6745f, -0.5890f, -0.6994f, -0.6286f, -0.6560f, -0.7225f, 0.6102f, 0.5987f, 0.5746f, 0.5433f, 0.5120f, 0.4879f, 0.4987f, 0.5272f, 0.5594f, 0.5880f, 0.6063f, 0.6012f, 0.5899f, 0.5664f, 0.5359f, 0.5055f, 0.4820f, 0.4928f, 0.5206f, 0.5520f, 0.5797f, 0.5975f, 0.5700f, 0.5595f, 0.5379f, 0.5102f, 0.4827f, 0.4618f, 0.4726f, 0.4979f, 0.5263f, 0.5513f, 0.5670f, 0.5188f, 0.5095f, 0.4911f, 0.4680f, 0.4454f, 0.4285f, 0.4393f, 0.4606f, 0.4841f, 0.5045f, 0.5170f, 0.4510f, 0.4433f, 0.4292f, 0.4122f, 0.3960f, 0.3844f, 0.3952f, 0.4112f, 0.4283f, 0.4426f, 0.4508f, 0.3714f, 0.3654f, 0.3564f, 0.3465f, 0.3379f, 0.3326f, 0.3434f, 0.3531f, 0.3626f, 0.3698f, 0.3730f, 0.2852f, 0.2812f, 0.2777f, 0.2755f, 0.2751f, 0.2766f, 0.2873f, 0.2903f, 0.2916f, 0.2911f, 0.2887f, -0.0139f, -0.0532f, 0.1417f, 0.1452f, 0.1474f, 0.1478f, 0.1463f, 0.1433f, 0.1394f, 0.1355f, 0.1326f, 0.1312f, 0.1318f, 0.1341f, 0.1377f, 0.2667f, 0.2632f, 0.2608f, 0.2603f, 0.2616f, 0.2646f, 0.2753f, 0.2768f, 0.2764f, 0.2742f, 0.2707f, 0.3535f, 0.3480f, 0.3402f, 0.3318f, 0.3249f, 0.3210f, 0.3318f, 0.3401f, 0.3480f, 0.3535f, 0.3555f, 0.4351f, 0.4276f, 0.4146f, 0.3990f, 0.3844f, 0.3740f, 0.3848f, 0.3996f, 0.4151f, 0.4280f, 0.4352f, 0.5057f, 0.4967f, 0.4792f, 0.4573f, 0.4359f, 0.4200f, 0.4308f, 0.4511f, 0.4734f, 0.4926f, 0.5043f, 0.5608f, 0.5505f, 0.5295f, 0.5026f, 0.4760f, 0.4558f, 0.4666f, 0.4912f, 0.5187f, 0.5428f, 0.5580f, 0.5964f, 0.5853f, 0.5620f, 0.5320f, 0.5020f, 0.4789f, 0.4897f, 0.5172f, 0.5481f, 0.5754f, 0.5928f, -0.2910f, -0.2091f, -0.2491f, -0.4564f, -0.2191f, -0.4587f, -0.2089f, -0.2857f, -0.2214f, -0.2432f, -0.2531f, -0.3275f, -0.2956f, -0.2115f, -0.3375f, -0.2392f, -0.3675f, -0.2810f, -0.3235f, -0.4686f, -0.3652f, -0.3576f, -0.3335f, -0.1990f, -0.3552f, -0.4663f, -0.2108f, -0.2233f, -0.2510f, -0.2209f, -0.2550f, -0.2928f, -0.2975f, -0.3353f, -0.3054f, -0.3394f, -0.3671f, -0.3694f, -0.4705f, -0.3803f, -0.4682f, 0.0861f, 0.0786f, 0.0826f, 0.0751f, 0.0890f, 0.0885f, 0.0725f, 0.0729f, 0.0877f, 0.0847f, 0.0770f, 0.0740f, 0.0809f };
        public static float[] vertY = { 0.2816f, 0.3400f, -0.4695f, -0.5996f, 0.4029f, 0.5299f, -0.3482f, -0.4097f, 0.2826f, 0.3309f, -0.5091f, -0.6088f, 0.4187f, 0.5208f, -0.4189f, -0.3730f, -0.0250f, -0.0144f, -0.0065f, -0.0031f, -0.0049f, -0.0116f, -0.0421f, -0.0479f, -0.0488f, -0.0444f, -0.0358f, 0.0567f, 0.0654f, 0.0682f, 0.0643f, 0.0547f, 0.0416f, 0.0110f, 0.0116f, 0.0186f, 0.0303f, 0.0440f, 0.1325f, 0.1395f, 0.1374f, 0.1267f, 0.1099f, 0.0909f, 0.0603f, 0.0669f, 0.0811f, 0.0995f, 0.1181f, 0.1972f, 0.2027f, 0.1965f, 0.1801f, 0.1571f, 0.1329f, 0.1024f, 0.1141f, 0.1344f, 0.1586f, 0.1813f, 0.2464f, 0.2508f, 0.2415f, 0.2206f, 0.1930f, 0.1649f, 0.1344f, 0.1500f, 0.1749f, 0.2036f, 0.2294f, 0.2768f, 0.2805f, 0.2692f, 0.2456f, 0.2151f, 0.1847f, 0.1542f, 0.1721f, 0.1999f, 0.2313f, 0.2591f, 0.2862f, 0.2897f, 0.2778f, 0.2534f, 0.2220f, 0.1908f, 0.1603f, 0.1790f, 0.2077f, 0.2399f, 0.2683f, -0.5998f, 0.5298f, -0.3522f, -0.3404f, -0.3159f, -0.2845f, -0.2533f, -0.2295f, -0.2185f, -0.2228f, -0.2415f, -0.2702f, -0.3025f, -0.3308f, -0.3488f, -0.3467f, -0.3288f, -0.3004f, -0.2682f, -0.2395f, -0.2208f, -0.2513f, -0.2825f, -0.3139f, -0.3383f, -0.3502f, -0.3346f, -0.3169f, -0.2894f, -0.2582f, -0.2306f, -0.2129f, -0.2434f, -0.2737f, -0.3039f, -0.3272f, -0.3383f, -0.3017f, -0.2848f, -0.2593f, -0.2311f, -0.2066f, -0.1915f, -0.2220f, -0.2497f, -0.2768f, -0.2972f, -0.3062f, -0.2503f, -0.2346f, -0.2124f, -0.1888f, -0.1692f, -0.1581f, -0.1886f, -0.2122f, -0.2345f, -0.2503f, -0.2560f, -0.1839f, -0.1697f, -0.1517f, -0.1341f, -0.1208f, -0.1149f, -0.1455f, -0.1638f, -0.1798f, -0.1896f, -0.1911f, -0.1071f, -0.0946f, -0.0815f, -0.0707f, -0.0648f, -0.0650f, -0.0955f, -0.1078f, -0.1164f, -0.1194f, -0.1160f, 0.1310f, 0.0640f, 0.1210f, 0.0601f, 0.0922f, -0.0197f, 0.0525f, -0.0569f, 0.0123f, -0.0456f, -0.0174f, -0.0469f, -0.0287f, -0.0159f, -0.0187f, 0.0928f, 0.0100f, 0.1028f, 0.0915f, 0.0085f, 0.0899f, -0.0181f, 0.1197f, 0.0243f, 0.0617f, 0.0883f, 0.0577f, 0.0176f, 0.1262f, 0.0975f, -0.0121f, 0.1363f, -0.0235f, 0.1249f, 0.1329f, -0.0134f, 0.0952f, 0.0153f, 0.0137f, 0.0151f, 0.0936f, 0.2651f, 0.2865f, 0.2830f, 0.2746f, 0.2045f, 0.2368f, 0.2188f, 0.2502f, 0.1758f, 0.1571f, 0.1638f, 0.1876f, 0.1528f };

        public override void Draw(float t)
        {
            // layer 1: concentric lines
            //float l1t = life * life / 16;
            //float[] r1big = { 0.8f, 0.6f, 0.4f };
            //float[] r1small = { 0.75f, 0.3f, 0.2f };
            //float[] r1rings = { 1.004f, 1.008f, 1.016f };
            //for (var layer = 0; layer < r1big.Length; layer++) for (var i = 0f; i < 1; i += 1f / 48)
            //{
            //    Vector2 point = DrawUtils.toPolar(r1big[layer], (l1t + i) * 2 * (float)Math.PI);
            //    Vector2 offset = DrawUtils.toPolar(r1small[layer], (l1t * r1rings[layer] + i) * 2 * (float)Math.PI - ((float)Math.PI * (layer / (r1big.Length - 1)) / 2));
            //    DrawUtils.ClipLineSegmentsInRect(Bounds, 
            //        DrawUtils.correctBounds(Bounds, point + offset), 
            //        DrawUtils.correctBounds(Bounds, point - offset), out Vector2 r1a, out Vector2 r1b);
            //    drawLine(r1a, r1b, DrawUtils.ColorFromHSV(i, 1, (layer + 1f) / r1big.Length));
            //}
            for (var x = Bounds.X + 4; x < Bounds.X + Bounds.Width - 20; x += 16) for (var y = Bounds.Y + 18; y < Bounds.Y + Bounds.Height - 20; y += 16)
            {
                float size = 0.8f - Math.Abs((life % 3f) - ((x - Bounds.X + y - Bounds.Y - 22) / 480f + 1));
                if (size < 0) continue;
                size = size * size;
                spriteBatch.Draw(Hacknet.Utils.white, new Rectangle(x + 8 - (int)(8 * size), y + 8 - (int)(8 * size), (int)(16 * size), (int)(16 * size)), new Rectangle?(), DrawUtils.ColorFromHSV((float)ZeroDayToolKit.rnd.NextDouble(), 0.3f, 0.3f), 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            }
            Color c;
            if (life < 5) c = os.brightLockedColor * (life / 5);
            else if (life < 10) c = new Color(
                MathUtils.lerp(os.brightLockedColor.R, 1, (life - 5) / 5),
                MathUtils.lerp(os.brightLockedColor.G, 1, (life - 5) / 5),
                MathUtils.lerp(os.brightLockedColor.B, 1, (life - 5) / 5));
            else if (done) c = os.brightUnlockedColor;
            else c = Color.White;
            for (var i = 0; i < (life * 3 * fade); i++)
            {
                int r = ZeroDayToolKit.rnd.Next(edgeA.Length);
                drawLine(DrawUtils.correctBounds(Bounds, new Vector2(
                    vertY[edgeA[r]] + ((float)ZeroDayToolKit.rnd.NextDouble() * 0.02f - 0.01f),
                    -vertX[edgeA[r]] + ((float)ZeroDayToolKit.rnd.NextDouble() * 0.02f - 0.01f))), DrawUtils.correctBounds(Bounds, new Vector2(
                    vertY[edgeB[r]] + ((float)ZeroDayToolKit.rnd.NextDouble() * 0.02f - 0.01f),
                    -vertX[edgeB[r]] + ((float)ZeroDayToolKit.rnd.NextDouble() * 0.02f - 0.01f))), c * fade);
            }
            spriteBatch.Draw(Hacknet.Utils.white, DrawUtils.correctBounds(Bounds, new Vector4(-1, -0.5f, 2, 0.68f)), (done ? os.brightUnlockedColor : os.lockedColor) * 0.5f * fade);
            TextItem.doFontLabel(DrawUtils.correctBounds(Bounds, new Vector2(-0.9f, -0.45f)), done ? "P R O C E S S" : "H A C K I N G", GuiData.smallfont, Color.White * fade);
            TextItem.doFontLabel(DrawUtils.correctBounds(Bounds, new Vector2(-0.85f, -0.25f)), done ? "F I N I S H E D" : "I N", GuiData.smallfont, Color.White * fade);
            TextItem.doFontLabel(DrawUtils.correctBounds(Bounds, new Vector2(-0.8f, -0.05f)), done ? "*kill this pls" : "P R O G R E S S . .", GuiData.smallfont, Color.White * fade);
            base.Draw(t);
        }
    }
}
