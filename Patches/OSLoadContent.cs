using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.LoadContent))] // reset data
    public class OSLoadContent
    {
        static void Prefix(OS __instance)
        {
            if (__instance.canRunContent)
            {
                Network.networks.Clear();
                Network.postLoadComputerCache.Clear();
                Network.tracker.Stop();
            }
        }
    }
}
