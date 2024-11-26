using Hacknet;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace ZeroDayToolKit.Patches
{
    public class DLCIntroExeInstantActivate
    {

        [HarmonyLib.HarmonyPatch]
        public class PatchConstructor
        {
            static MethodBase TargetMethod()
            {
                return typeof(DLCIntroExe).GetConstructor(new Type[] { typeof(Rectangle), typeof(OS), typeof(string[]) });
            }

            static void Postfix(DLCIntroExe __instance, Rectangle location, OS operatingSystem, string[] p)
            {
                if (p.Length > 1 && p[1].ToLower() == "-i" && !__instance.os.Flags.HasFlag("KaguyaTrialComplete"))
                {
                    __instance.State = DLCIntroExe.IntroState.SpinningUp;
                    __instance.TimeInThisState = 0.0f;
                    MusicManager.stop();
                    MusicManager.playSongImmediatley("DLC\\Music\\snidelyWhiplash");
                    __instance.os.mailicon.isEnabled = false;
                    __instance.os.thisComputer.links.Clear();
                    __instance.os.traceCompleteOverrideAction += new Action(__instance.PlayerLostToTraceTimer);
                    __instance.OSTraceTimerOverrideActive = true;
                }
            }
        }
    }
}
