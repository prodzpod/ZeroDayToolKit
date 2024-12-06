using Hacknet;
using Pathfinder.Util;
using System.Collections.Generic;
using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Conditions
{
    public class SASendEvent : Pathfinder.Action.DelayablePathfinderAction
    {
        public static List<string> Actions = [];
        public static List<string> _Actions = [];
        [XMLStorage]
        public string signal;
        [XMLStorage]
        public string Signal;
        public override void Trigger(OS os)
        {
            if (!Actions.Contains(signal ?? Signal)) Actions.Add(signal ?? Signal);
        }
    }
}
