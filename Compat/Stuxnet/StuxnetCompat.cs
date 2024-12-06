using BepInEx.Hacknet;
using Hacknet;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDayToolKit.Commands;

namespace ZeroDayToolKit.Compat.Stuxnet
{
    public class StuxnetCompat
    {
        public static bool Enabled => HacknetChainloader.Instance.Plugins.ContainsKey("autumnrivers.stuxnet");
        public static bool Applied = false;
        public static Dictionary<string, string> RadioBinaries = []; // songid -> binary
        public static void Init()
        {
            if (Applied) return;
            Applied = true;
            ZeroDayToolKit.Instance.Log.LogInfo("Stuxnet Detected: Loading Radio Compat");
            ZeroDayToolKit.Instance.Log.LogDebug("Patching " + typeof(RadioFile));
            ZeroDayToolKit.Instance.HarmonyInstance.PatchAll(typeof(RadioFile));
            ZeroDayCommand.Add("radio", RadioCommand.Trigger, "[FILE]", "registers the file to Radio V3.");
        }
        public static string GetRadio(string songID)
        {
            if (!RadioBinaries.ContainsKey(songID))
            {
                Random _r = Hacknet.Utils.random;
                Hacknet.Utils.random = new Random(songID.GetHashCode());
                ZeroDayToolKit.Instance.Log.LogInfo($"Loading radio for {songID} ({songID.GetHashCode()})");
                RadioBinaries[songID] = Computer.generateBinaryString(500);
                Hacknet.Utils.random = _r;
            }
            return RadioBinaries[songID];
        }
        public static bool IsRadioFile(string data) => RadioBinaries.Values.Any(x => x == data.Trim());
        public static void InstallRadio(OS os, string data) => RadioCommand.Install(os, data);
    }
}
