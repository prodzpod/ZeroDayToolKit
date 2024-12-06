using System;
using System.Linq;
using System.Threading;
using Hacknet;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Source : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: source [FILE]"); return; }
            Folder currentFolder = Programs.getCurrentFolder(os);
            var entry = ComUtils.GetPath(os, args[1], os.navigationPath);
            if (entry.File == null) { os.write("File does not exist"); return; }
            os.write("Executing " + args[1]);
            foreach (var _command in entry.File.data.Split('\n')) {
                var command = _command;
                if (command.StartsWith("#")) continue; // comment
                for (int k = 0; k < Math.Min(10, args.Length); k++) command = command.Replace("$" + k, args[k]);
                ProgramRunner.ExecuteProgram(os, command.Replace("$#", args.Length.ToString()).Replace("$*", args.Range(1).Join(" ")).Trim().Split(' '));
                Thread.Sleep(200);
            }
            os.write("File Executed");
        }
    }
}
