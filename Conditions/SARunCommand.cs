using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SARunCommand : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string command;

        public override void Trigger(OS os)
        {
            os.runCommand(command);
        }
    }
}
