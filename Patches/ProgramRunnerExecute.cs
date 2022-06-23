using Hacknet;

using ZeroDayToolKit.Conditions;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(ProgramRunner), nameof(ProgramRunner.ExecuteProgram))] // disable commands
    public class ProgramRunnerExecute
    {
        static bool Prefix(object os_object, string[] arguments, ref bool __result)
        {
            OS os = (OS)os_object;
            string[] strArray = arguments;
            if (ZeroDayConditions.disabledCommands.Contains(strArray[0].ToLower()))
            {
                os.write("Fatal error has occured while executing this command, Command aborted.");
                __result = false;
                return false; // return now
            }
            return true; // otherwise continue executing
        }
    }
}
