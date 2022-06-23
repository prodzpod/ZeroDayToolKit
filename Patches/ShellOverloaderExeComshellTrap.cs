using Hacknet;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace ZeroDayToolKit.Patches
{
    [HarmonyPatch(typeof(ShellOverloaderExe), nameof(ShellOverloaderExe.RunShellOverloaderExe))]
    public class ShellOverloaderExeComshellTrap
    {
        internal static bool Prefix(string[] args, object osObj, Computer target)
        {
            OS os = (OS)osObj;
            if (args.Length > 1 && args[1].ToLower() == "-t")
            {
                for (int index = 0; index < os.exes.Count; ++index)
                {
                    if (os.exes[index] is ShellExe ex)
                    {
                        ex.state = 2;
                        ex.targetRamUse = ShellExe.TRAP_RAM_USE;
                        ex.destinationIP = ex.os.connectedComp == null ? ex.os.thisComputer.ip : ex.os.connectedComp.ip;
                        if (ex.destComp == null || ex.destComp.ip != ex.destinationIP)
                            ex.compThisShellIsRunningOn.log("#SHELL_TrapAcive");
                        ex.destComp = Programs.getComputer(ex.os, ex.destinationIP);
                        ex.destCompIndex = ex.os.netMap.nodes.IndexOf(ex.destComp);
                        ex.destComp.forkBombClients(ex.targetIP);
                        ex.completedAction(2);
                        ex.compThisShellIsRunningOn.log("#SHELL_TrapActivate_:_ConnectionsFlooded");
                    }
                }
                return false; // return now
            }
            return true; // otherwise continue executing
        }

        internal static void ILManipulator(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.Before, x => x.MatchLdcI4(8), x => x.MatchNewarr(typeof(string)));
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, 10);
            c.GotoNext(MoveType.Before, x => x.MatchLdstr(")]"));
            c.Emit(OpCodes.Ldstr, ")] [-t (");
            c.Emit(OpCodes.Stelem_Ref);
            c.Emit(OpCodes.Ldloc, 7);
            c.Emit(OpCodes.Ldc_I4, 8);
            c.EmitDelegate<Func<string>>(() => LocaleTerms.Loc("Trap"));
            c.Emit(OpCodes.Stelem_Ref);
            c.Emit(OpCodes.Ldloc, 7);
            c.Emit(OpCodes.Ldc_I4, 9);
        }
    }
}
