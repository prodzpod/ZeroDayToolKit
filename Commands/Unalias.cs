using System;
using System.Linq;
using Hacknet;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Unalias : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: unalias [key]"); return; }
            if (ZeroDayConditions.aliases.Remove(args[1])) os.write($"Removed alias " + args[1]);
            else os.write(args[1] + " is not a registered alias");
            ProgramList.init();
        }
    }
}
