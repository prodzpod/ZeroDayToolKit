using Hacknet;
using System.Text.RegularExpressions;


namespace ZeroDayToolKit.Compat.Stuxnet
{
    [HarmonyLib.HarmonyPatch(typeof(ComputerLoader), nameof(ComputerLoader.filter))]
    public class RadioFile
    {
        public static void Postfix(ref string __result)
        { __result = new Regex("#STUXNET_RADIO:[^#]+#").Replace(__result, m => StuxnetCompat.GetRadio(m.Captures[0].Value.Substring("#STUXNET_RADIO:".Length, m.Captures[0].Value.Length - "#STUXNET_RADIO:#".Length).Trim())); }
    }
}