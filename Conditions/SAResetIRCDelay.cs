using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SAResetIRCDelay : Pathfinder.Action.PathfinderAction
    {
        [XMLStorage]
        public string target;
        [XMLStorage]
        public string Target;

        public override void Trigger(object os_obj)
        {
            OS os = (OS)os_obj;
            ZeroDayConditions.times[target ?? Target] = os.lastGameTime.TotalGameTime;
        }
    }
}
