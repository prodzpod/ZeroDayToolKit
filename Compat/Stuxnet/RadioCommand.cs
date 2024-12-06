using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Hacknet;
using Hacknet;
using Hacknet.Extensions;
using Newtonsoft.Json;
using Stuxnet_HN;
using Stuxnet_HN.Executables;
using ZeroDayToolKit.Commands;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Compat.Stuxnet
{
    public class RadioCommand : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: radio [FILE]"); return; }
            Folder currentFolder = Programs.getCurrentFolder(os);
            var entry = ComUtils.GetPath(os, args[1], os.navigationPath);
            if (entry.File == null) { os.write("File does not exist"); os.commandInvalid = true; return; }
            Install(os, entry.File.data);
        }
        public static void Install(OS os, string data)
        {
            var id = StuxnetCompat.RadioBinaries.Keys.First(x => StuxnetCompat.RadioBinaries[x] == data);
            var name = GetSongName(id);
            if (string.IsNullOrWhiteSpace(name)) { os.write("File is not a valid audio file"); os.commandInvalid = true; return; }
            StuxnetCore.unlockedRadio.Add(id);
            os.write($"Successfully added {name} to RADIO_V3");
        }
        public static string GetSongName(string id) {
            string ret = "";
            if (id == null) return ret;
            string folderPath = ExtensionLoader.ActiveExtensionInfo.FolderPath;
            string path = folderPath + "/radio.json";
            if (!File.Exists(path)) return ret;
            StreamReader streamReader = new(path);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            Dictionary<string, SongEntry> dictionary = JsonConvert.DeserializeObject<Dictionary<string, SongEntry>>(text);
            if (!dictionary.ContainsKey(id)) return ret;
            return $"{dictionary[id].artist} - {dictionary[id].title}";
        }
    }
}
