using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.crash))] // set recentreboot & recentcrash, snooze tracev2 if possible
    public class ComputerCrash
    {
        static void Postfix(Computer __instance)
        {
            Network.recentReboot = __instance;
            Network.recentCrash = __instance;
            if (Network.tracker.network?.head == __instance) Network.tracker.RebootHead();
        }
    }
}
