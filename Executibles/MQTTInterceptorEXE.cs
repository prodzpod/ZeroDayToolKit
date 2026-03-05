namespace ZeroDayToolKit.Executibles
{
    public class MQTTInterceptorEXE : ZeroDayEXE
    {
        public MQTTInterceptorEXE() : base()
        {
            Init(1883, 20f, 375, "mqttpwn:r63", ". . . MQTTPWN . . . // Thank you for using our service.", "");
        }

        public override void Draw(float t)
        {
            base.Draw(t);
        }
    }
}
