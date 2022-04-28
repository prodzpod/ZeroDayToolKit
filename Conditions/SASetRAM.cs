using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SASetRAM : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public float ram;

        public override void Trigger(OS os)
        {
            os.ramAvaliable += (int)ram - os.totalRam;
            os.totalRam = (int)ram - (OS.TOP_BAR_HEIGHT + 2);
        }
    }
}
