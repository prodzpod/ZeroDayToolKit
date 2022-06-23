using Hacknet;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ZeroDayToolKit.Patches
{
    [HarmonyPatch(typeof(ShellOverloaderExe), nameof(ShellOverloaderExe.RunShellOverloaderExe))]
    public class ShellOverloaderExeComshellTypoFix
    {
        internal static void ILManipulator (ILContext il)
        {
            var c = new ILCursor(il);
            while (true)
            {
                if (c.TryGotoNext(x => x.MatchLdstr("ConShell ")))
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldstr, "ComShell ");
                }
                else if (c.TryGotoNext(x => x.MatchLdstr(" ConShell [-")))
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldstr, " ComShell [-");
                }
                else break;
            }
        }
    }
}
