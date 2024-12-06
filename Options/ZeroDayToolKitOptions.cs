using BepInEx.Configuration;
using Pathfinder.Event;
using Pathfinder.Event.Options;
using Pathfinder.Options;
using System;

namespace ZeroDayToolKit.Options
{
    public static class ZeroDayToolKitOptions
    {
        public const string OPTION_TAG = "ZeroDayToolKit";
        internal static OptionSlider SFXVolume = new("Sound Effect Volume", "");
        public static void Initialize()
        {
            OptionsManager.AddOption(OPTION_TAG, SFXVolume);
            EventManager<CustomOptionsSaveEvent>.AddHandler(new Action<CustomOptionsSaveEvent>(onOptionsSave));
            initConfig();
        }

        private static void initConfig()
        {
            ConfigEntry<float> entry = ZeroDayToolKit.Config.Bind(OPTION_TAG, "SFXVolume", 1f, "Sound Effect Volume");
            SFXVolume.Value = entry.Value;
        }

        private static void onOptionsSave(CustomOptionsSaveEvent _)
        {
            ConfigEntry<float> entry;
            ZeroDayToolKit.Config.TryGetEntry(OPTION_TAG, "SFXVolume", out entry);
            entry.Value = SFXVolume.Value;
            ZeroDayToolKit.Config.Save();
        }
    }
}
