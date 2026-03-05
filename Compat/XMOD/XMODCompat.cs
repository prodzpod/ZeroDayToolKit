namespace ZeroDayToolKit.Compat.XMOD
{
    public class XMODCompat: ModCompats
    {
        public const string ID = "tenesiss.XMOD";
        public override string _ID => ID;
        public override void Init()
        {
            ZeroDayToolKit.Instance.Log.LogInfo("XMOD Detected: Loading Feature Compat");
        }
    }
}
