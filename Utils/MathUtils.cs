namespace ZeroDayToolKit.Utils
{
    public class MathUtils
    {
        public static float Ratio(float a1, float b1, float a2, float b2, float a3)
        {
            return b1 + ((a3 - a1) * (b1 - b2) / (a1 - a2));
        }
    }
}
