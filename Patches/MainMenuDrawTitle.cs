using Microsoft.Xna.Framework;
using Hacknet;
using Hacknet.Gui;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(MainMenu), nameof(MainMenu.DrawBackgroundAndTitle))] // funny text :)
    public class MainMenuDrawTitle
    {
        static void Prefix(MainMenu __instance)
        {
            TextItem.doFontLabel(new Vector2(__instance.State == MainMenu.MainMenuState.Normal ? 634 : 496, 200), "+ ZeroDayToolKit " + ZeroDayToolKit.ModVer, GuiData.smallfont, new Color(0, 255, 255));
        }
    }
}
