using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Programs), nameof(Programs.connect))] // connections+, reset recent&#&
    public class ProgramsConnect
    {
        static void Postfix(OS os)
        {
            ProgramsDisconnect.Postfix(os);
            // nulling everything so unrelated hacking from ages ago dont trigger new conditions
            Network.connections++;
            for (int i = 0; i < Network.afterCompleteTriggers.Count; i++)
            {
                AfterCompleteTrigger trigger = Network.afterCompleteTriggers[i];
                if (!trigger.TryTrigger())
                {
                    Network.afterCompleteTriggers.Remove(trigger);
                    i--;
                }
            }
        }
    }
}
