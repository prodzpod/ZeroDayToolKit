using Microsoft.Xna.Framework.Audio;
using ZeroDayToolKit.Options;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(SoundEffect), nameof(SoundEffect.Play), typeof(float), typeof(float), typeof(float))] // set recentreboot
    public class SoundEffectVolume
    {
        static void Prefix(ref float volume, float pitch, float pan)
        {
            volume *= ZeroDayToolKitOptions.SFXVolume.Value;
        }
    }
}
