using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SADisableCommand : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string command;

        public override void Trigger(OS os)
        {
            ZeroDayConditions.disabledCommands.Add(command.ToLower());
        }
    }
}
