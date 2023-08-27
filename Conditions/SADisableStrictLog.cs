using Hacknet;
using Pathfinder.Util;
using ZeroDayToolKit.Patches;

namespace ZeroDayToolKit.Conditions
{
    public class SADisableStrictLog : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string targetComp;

        public override void Trigger(OS os)
        {
            Computer c = Programs.getComputer(os, targetComp);
            if (TrackerCheckLogs.stricts.Contains(c)) TrackerCheckLogs.stricts.Remove(c);
        }
    }
}
