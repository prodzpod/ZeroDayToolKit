using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SASetNumberOfChoices : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public int choices;

        public override void Trigger(OS os)
        {
            ZeroDayConditions.choice = choices;
        }
    }
}
