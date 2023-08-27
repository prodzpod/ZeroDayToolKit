using Hacknet;
using Pathfinder.Command;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SAEnableCommand : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string command;

        public override void Trigger(OS os)
        {
            ZeroDayConditions.disabledCommands.Remove(command.ToLower());
            Helpfile.init();
            ProgramList.init();
        }
    }
}
