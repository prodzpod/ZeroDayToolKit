using Hacknet;
using Hacknet.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(GuiData), nameof(GuiData.getFilteredKeys))]
    public class IME
    {
        public static void ILManipulator(ILContext il)
        {
            ILCursor c = new(il);
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
    [HarmonyLib.HarmonyPatch(typeof(TextInputHook), nameof(TextInputHook.OnTextInput))]
    public class IMESpecialKeyCheck
    {
        public static bool Prefix(char c)
        {
            if (c == '\u007F') return false;
            return true;
        }
    }

}
