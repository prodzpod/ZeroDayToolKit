using Hacknet;
using HarmonyLib;
using Pathfinder.Command;
using Pathfinder.Util;
using System.Linq;
using ZeroDayToolKit.Commands;

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
            var c = (command ?? Command).Trim();
            if (ZeroDayConditions.defaultAliases.ContainsKey(c)) c = ZeroDayConditions.defaultAliases[c].Split(' ')[0];
            ZeroDayToolKit.Instance.Log.LogInfo("Disabling Command: " + c);
            if (!ZeroDayConditions.disabledCommands.Contains(c))
            {
                ZeroDayConditions.disabledCommands.Add(c);
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
                foreach (var alias in ZeroDayConditions.aliases)
                    if (ZeroDayConditions.disabledCommands.All(x => Alias.FindAlias(alias.Value, x) == -1))
                        ProgramList.programs.Add(alias.Key);
            }
        }
    }
}
