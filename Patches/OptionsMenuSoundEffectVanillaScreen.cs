using Hacknet;
using Hacknet.Gui;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ZeroDayToolKit.Options;
namespace ZeroDayToolKit.Patches
{
    [HarmonyPatch(typeof(OptionsMenu), nameof(OptionsMenu.Draw), typeof(GameTime))] // set recenthostile, activate tracev2 if possible
    public class OptionsMenuSoundEffectVanillaScreen
    {
        internal static void ILManipulator(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, x => x.MatchLdstr("Music Volume"));
            c.GotoNext(MoveType.After, x => x.MatchLdcR4(0.001f));
            c.GotoNext(MoveType.After, x => x.MatchNop());

            c.Emit(OpCodes.Ldc_R4, 100f);
            c.Emit(OpCodes.Ldloc_3);
            c.Emit(OpCodes.Ldc_I4, 32);
            c.Emit(OpCodes.Add);
            c.Emit(OpCodes.Dup);
            c.Emit(OpCodes.Stloc_3);
            c.Emit(OpCodes.Conv_R4);
            c.Emit(OpCodes.Newobj, AccessTools.Constructor(typeof(Vector2), new[] { typeof(float), typeof(float) }));
            c.Emit(OpCodes.Ldstr, "Sound Effects Volume");
            c.Emit(OpCodes.Call, AccessTools.Method(typeof(LocaleTerms), nameof(LocaleTerms.Loc), new[] { typeof(string) }));
            c.Emit(OpCodes.Ldloca, 10);
            c.Emit(OpCodes.Initobj, typeof(Color?));
            c.Emit(OpCodes.Ldloc, 10);
            c.Emit(OpCodes.Ldloc, 4);
            c.Emit(OpCodes.Call, AccessTools.Method(typeof(TextItem), nameof(TextItem.doLabel), new[] { typeof(Vector2), typeof(string), typeof(Color?), typeof(float) }));
            c.Emit(OpCodes.Nop);

            c.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(ZeroDayToolKitOptions), nameof(ZeroDayToolKitOptions.SFXVolume)));
            c.Emit(OpCodes.Ldc_I4, ZeroDayToolKitOptions.SFXVolume.ButtonID);
            c.Emit(OpCodes.Ldc_I4, 100);
            c.Emit(OpCodes.Ldloc_3);
            c.Emit(OpCodes.Ldc_I4, 34);
            c.Emit(OpCodes.Add);
            c.Emit(OpCodes.Dup);
            c.Emit(OpCodes.Stloc_3);
            c.Emit(OpCodes.Ldc_I4, 210);
            c.Emit(OpCodes.Ldc_I4, 30);
            c.Emit(OpCodes.Ldc_R4, 1f);
            c.Emit(OpCodes.Ldc_R4, 0f);
            c.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(ZeroDayToolKitOptions), nameof(ZeroDayToolKitOptions.SFXVolume)));
            c.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(OptionSlider), nameof(OptionSlider.Value)));
            c.Emit(OpCodes.Ldc_R4, 0.001f);
            c.Emit(OpCodes.Call, AccessTools.Method(typeof(SliderBar), nameof(SliderBar.doSliderBar), new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(float) }));
            c.Emit(OpCodes.Stfld, AccessTools.Field(typeof(OptionSlider), nameof(OptionSlider.Value)));
            c.Emit(OpCodes.Nop);
        }
    }
}
