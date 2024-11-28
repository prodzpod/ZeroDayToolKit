using Hacknet;
using Hacknet.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(GuiData), nameof(GuiData.getFilteredKeys))]
    public class IME
    {
        public static void ILManipulator(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(x => x.MatchBrtrue(out _));
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldloc, 1);
            c.Emit(OpCodes.Ldloc, 3);
            c.EmitDelegate<Func<string, int, bool>>((buffer, i) =>
            {
                var ch = buffer[i];
                // todo: filter better
                return !(ch >= ' ');
            });
        }
    }
}
