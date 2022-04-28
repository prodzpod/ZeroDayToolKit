using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Computer), nameof(Computer.hostileActionTaken))] // set recenthostile, activate tracev2 if possible
    public class ComputerHostileActionTaken
    {
        static void Postfix(Computer __instance)
        {
            Network.recentHostileActionTaken = __instance.os.connectedComp;
            foreach (Network network in Network.networks.Values) 
                if (network.tail.Contains(__instance) && Network.tracker.network != network) 
                    Network.tracker.Start(__instance.os, network, __instance);
        }
    }
}
