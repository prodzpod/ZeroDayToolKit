using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.drawModules))] // hook tracker draw
    public class OSDrawModules
    {
        static void Postfix()
        {
            Network.tracker.Draw(GuiData.spriteBatch);
        }
    }
}
