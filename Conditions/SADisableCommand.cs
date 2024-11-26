using Hacknet;
using HarmonyLib;
using Pathfinder.Command;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SADisableCommand : Pathfinder.Action.DelayablePathfinderAction
    {
        [XMLStorage]
        public string command;
        [XMLStorage]
        public string Command;

        public override void Trigger(OS os)
        {
            if (ZeroDayConditions.disabledCommands.Contains(command ?? Command))
            {
                ZeroDayConditions.disabledCommands.Add((command ?? Command).ToLower());
                Helpfile.init();
                ProgramList.init();
            }
        }

        [HarmonyPatch(typeof(CommandManager), "RebuildAutoComplete")]
        public class RemoveDisabledCommandFromAutocomplete
        {
            public static void PostFix()
            {
                foreach (var command in ZeroDayConditions.disabledCommands) ProgramList.programs.Remove(command);
            }
        }
    }
}
