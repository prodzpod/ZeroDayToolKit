using Hacknet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZeroDayToolKit.Patches
{
    public class SequencerExeInstantActivate
    {
        public static List<SequencerExe> queue = new List<SequencerExe>();

        [HarmonyLib.HarmonyPatch]
        public class PatchConstructor
        {
            static MethodBase TargetMethod()
            {
                Console.WriteLine(typeof(SequencerExe).GetConstructor(new Type[] { typeof(Rectangle), typeof(OS), typeof(string[]) }));
                return typeof(SequencerExe).GetConstructor(new Type[] { typeof(Rectangle), typeof(OS), typeof(string[]) });
            }

            static void Postfix(SequencerExe __instance, Rectangle location, OS operatingSystem, string[] p)
            {
                if (p.Length > 1 && p[1].ToLower() == "-i") queue.Add(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SequencerExe), nameof(SequencerExe.LoadContent))]
        public class PatchLoadContent
        {
            static void Postfix(SequencerExe __instance)
            {
                if (queue.Contains(__instance))
                {
                    queue.Remove(__instance);
                    if (__instance.os.TraceDangerSequence.IsActive)
                    {
                        __instance.os.write("SEQUENCER ERROR: OS reports critical action already in progress.");
                    }
                    else // go immediately
                    {
                        __instance.stateTimer = 0.0f;
                        __instance.state = SequencerExe.SequencerExeState.SpinningUp;
                        __instance.bars.MinLineChangeTime = 0.1f;
                        __instance.bars.MaxLineChangeTime = 1f;
                        __instance.originalTheme = ThemeManager.currentTheme;
                        MusicManager.FADE_TIME = 0.6f;
                        __instance.oldSongName = MusicManager.currentSongName;
                        MusicManager.transitionToSong("Music\\Roller_Mobster_Clipped");
                        MediaPlayer.IsRepeating = false;
                        __instance.targetComp = Programs.getComputer(__instance.os, __instance.targetID);
                        ((WebServerDaemon)__instance.targetComp.getDaemon(typeof(WebServerDaemon)))?.LoadWebPage();
                    }
                }
            }
        }
    }
}
