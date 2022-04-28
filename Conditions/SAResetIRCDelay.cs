using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SAResetIRCDelay : Pathfinder.Action.PathfinderAction
    {
        [XMLStorage]
        public string target;

        public override void Trigger(object os_obj)
        {
            OS os = (OS)os_obj;
            ZeroDayCondition.times[target] = os.lastGameTime.TotalGameTime;
        }
    }
}
