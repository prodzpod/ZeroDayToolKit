using Microsoft.Xna.Framework;
using Hacknet;

namespace ZeroDayToolKit.Executibles
{
    public class MQTTInterceptorEXE : ZeroDayEXE
    {
        public MQTTInterceptorEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(1883, 20f, 375, "mqttpwn:r63", ". . . MQTTPWN . . . // Thank you for using our service.", "");
        }

        public override void Draw(float t)
        {
            base.Draw(t);
        }
    }
}
