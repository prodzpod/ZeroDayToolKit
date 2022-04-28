using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.reboot))] // set recentreboot
    public class ComputerReboot
    {
        static void Postfix(Computer __instance)
        {
            Network.recentReboot = __instance;
        }
    }
}
