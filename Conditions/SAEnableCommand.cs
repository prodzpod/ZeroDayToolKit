using Hacknet;
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
            var c = (command ?? Command).Trim();
            if (ZeroDayConditions.defaultAliases.ContainsKey(c)) c = ZeroDayConditions.defaultAliases[c].Split(' ')[0];
            ZeroDayToolKit.Instance.Log.LogInfo("Enabling Command: " + c);
            if (ZeroDayConditions.disabledCommands.Remove(c))
            {
                Helpfile.init();
                ProgramList.init();
            }
        }
    }
}
