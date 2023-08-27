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

        public override void Trigger(OS os)
        {
            ZeroDayConditions.disabledCommands.Add(command.ToLower());
            Helpfile.init();
            ProgramList.init();
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
