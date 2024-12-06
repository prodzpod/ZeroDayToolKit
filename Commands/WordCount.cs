using System;
using System.Linq;
using Hacknet;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class WordCount : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: wc [FILE]"); return; }
            Folder currentFolder = Programs.getCurrentFolder(os);
            var entry = ComUtils.GetPath(os, args[1], os.navigationPath);
            if (entry.File == null) { os.validCommand = false; os.write("File does not exist"); return; }
            var data = entry.File.data;
            int lines = 0, words = 0;
            int i, idx = data.IndexOf('\n');
            while (idx != -1)
            {
                lines++;
                i = idx + 1;
                idx = data.IndexOf('\n', i);
            }
            while (!string.IsNullOrWhiteSpace(data))
            {
                words++;
                int idx2 = data.IndexOf(' '), idx3 = data.IndexOf('\n');
                if (idx2 == -1) idx2 = data.Length; if (idx3 == -1) idx3 = data.Length;
                data = data.Substring(Math.Min(idx2, idx3));
                data = data.TrimStart();
            }
            os.write($"Lines: {lines}\nWords: {words}\nBytes: {entry.File.data.Length}\nFilename: {entry.File.name}");
        }
    }
}
