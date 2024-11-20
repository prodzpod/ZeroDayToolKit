using Hacknet;
using Pathfinder.Command;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SAEnableCommand : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string command;
        [XMLStorage]
        public string Command;

        public override void Trigger(OS os)
        {
            ZeroDayConditions.disabledCommands.Remove((command ?? Command).ToLower());
            Helpfile.init();
            ProgramList.init();
        }
    }
}
