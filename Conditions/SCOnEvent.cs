using Hacknet;
using Pathfinder.Util;
using System;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnEvent : Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public string signal = null;
        [XMLStorage]
        public string Signal = null;
        [XMLStorage]
        public string requiredFlags = null;
        [XMLStorage]
        public string RequiredFlags = null;
        [XMLStorage]
        public string doesNotHaveFlags = null;
        [XMLStorage]
        public string DoesNotHaveFlags = null;
        public override bool Check(object os_obj)
        {
            var requiredFlags = this.requiredFlags ?? RequiredFlags;
            var doesNotHaveFlags = this.doesNotHaveFlags ?? DoesNotHaveFlags;
            OS os = (OS)os_obj;
            if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
            if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
            var s = signal ?? Signal;
            return SASendEvent._Actions.Contains(s);
        }
    }
}
