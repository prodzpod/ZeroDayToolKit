using Hacknet;
using Pathfinder.Command;
using Pathfinder.Event.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDayToolKit.Commands;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(ProgramRunner), nameof(ProgramRunner.ExecuteProgram))] // disable commands
    public class ProgramRunnerExecute
    {
        static bool Prefix(object os_object, ref string[] arguments, ref bool __result)
        {
            OS os = (OS)os_object;
            if (ZeroDayConditions.disabledCommands.Contains(arguments[0]))
            {
                os.write("Fatal error has occured while executing this command, Command aborted.");
                __result = false;
                return false; // return now
            }
            return true; // otherwise continue executing
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(CommandManager), "OnCommandExecute")]
    public class DisableCustomCommand
    {
        public static bool Prefix(CommandExecuteEvent args) => !ZeroDayConditions.disabledCommands.Contains(args.Args[0]);
    }
}
