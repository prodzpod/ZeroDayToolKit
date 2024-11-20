using System;
using Hacknet;
using Pathfinder.Util;

using ZeroDayToolKit.TraceV2;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnCrash : Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public string requiredFlags = null;
        [XMLStorage]
        public string RequiredFlags = null;
        [XMLStorage]
        public string doesNotHaveFlags = null;
        [XMLStorage]
        public string DoesNotHaveFlags = null;
        [XMLStorage]
        public string targetComp = null;
        [XMLStorage]
        public string TargetComp = null;
        [XMLStorage]
        public string targetNetwork = null;
        [XMLStorage]
        public string TargetNetwork = null;

        public override bool Check(object os_obj)
        {
            var requiredFlags = this.requiredFlags ?? RequiredFlags;
            var doesNotHaveFlags = this.doesNotHaveFlags ?? DoesNotHaveFlags;
            var targetComp = this.targetComp ?? TargetComp;
            var targetNetwork = this.targetNetwork ?? TargetNetwork;
            OS os = (OS)os_obj;
            Computer c = ComUtils.getComputer(os);
            if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
            if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
            if (targetComp != null && c != Programs.getComputer(os, targetComp)) return false;
            if (targetNetwork != null && Network.networks.ContainsKey(targetNetwork) && Network.networks[targetNetwork].tail.Contains(c)) return false;
            return Network.recentCrash == c;
        }
    }
}
