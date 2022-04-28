using Microsoft.Xna.Framework;
using Hacknet;

using ZeroDayToolKit.TraceV2;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.Update))] // hook tracker update
    public class OSUpdate
    {
        static void Postfix(OS __instance, GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (__instance.canRunContent && __instance.isLoaded) Network.tracker.Update(totalSeconds);
        }
    }
}
