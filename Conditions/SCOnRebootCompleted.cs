using System;
using System.Collections.Generic;
using Hacknet;
using Pathfinder.Util;

using ZeroDayToolKit.TraceV2;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnRebootCompleted : Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public bool RequireLogsOnSource = false;
        [XMLStorage]
        public bool RequireSourceIntact = false;
        [XMLStorage]
        public string requiredFlags = null;
        [XMLStorage]
        public string RequiredFlags = null;
        [XMLStorage]
        public string doesNotHaveFlags = null;
        [XMLStorage]
        public string DoesNotHaveFlags = null;
        [XMLStorage]
        public string targetNetwork = null;
        [XMLStorage]
        public string TargetNetwork = null;

        public override bool Check(object os_obj)
        {
            var requiredFlags = this.requiredFlags ?? RequiredFlags;
            var doesNotHaveFlags = this.doesNotHaveFlags ?? DoesNotHaveFlags;
            var targetNetwork = this.targetNetwork ?? TargetNetwork;
            OS os = (OS)os_obj;
            if (targetNetwork != null || !Network.networks.ContainsKey(targetNetwork)) return false;
            if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
            if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
            List<Computer> c = Network.networks[targetNetwork].tail;
            if (RequireLogsOnSource) foreach (Computer temp in c) if (ComUtils.hasLogOnSource(os, temp)) return false;
            if (RequireSourceIntact) foreach (Computer temp in c) if (ComUtils.isSourceIntact(os, temp)) return false;
            return Network.recentRebootCompleted != Network.networks[targetNetwork];
        }
    }
}
