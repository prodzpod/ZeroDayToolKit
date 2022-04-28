using Hacknet;

using ZeroDayToolKit.TraceV2;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Programs), nameof(Programs.disconnect))] // reset recent&#&
    public class ProgramsDisconnect
    {
        public static void Postfix(OS os)
        {
            Network.recentReboot = null;
            Network.recentRebootCompleted = null;
            Network.recentHostileActionTaken = null;
            // nulling everything so unrelated hacking from ages ago dont trigger new conditions
            if (Network.tracker.active == 1)
            {
                bool tracked = false;
                foreach (Computer c in Network.tracker.network.tail) if (ComUtils.hasLogOnSource(os, c)) tracked = true;
                if (!tracked) Network.tracker.RebootComplete();
            }
        }
    }
}
