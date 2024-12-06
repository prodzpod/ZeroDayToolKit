using System;
using System.Linq;
using Hacknet;
using HarmonyLib;
using Pathfinder.Command;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Alias : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 3) { os.validCommand = false; os.write("Usage: alias [from] [to]"); return; }
            ZeroDayConditions.aliases[args[1]] = args.Range(2).Join(" ");
            os.write($"Set {args[1]} to {args.Range(2).Join(" ")}");
            ProgramList.init();
        }

        public static int FindAlias(string str, string substr)
        {
            substr = substr.Trim();
            if (str == substr || str.StartsWith(substr + " ")) return 0;
            if (str.EndsWith(" " + substr)) return str.Length - substr.Length;
            var idx = str.IndexOf(" " + substr + " ");
            return idx != -1 ? idx + 1 : -1;
        }
    }
}
