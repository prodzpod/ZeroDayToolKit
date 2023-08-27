using System.Linq;
using Hacknet;
using ZeroDayToolKit.Patches;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Man : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: hostname [-i]"); }
            else
            {
                var cmd = args[1].ToLower();
                if (BetterHelp.Descriptions.ContainsKey(cmd)) os.write("Usage: " + cmd + " " + BetterHelp.Usages[cmd] + "\n  " + BetterHelp.Descriptions[cmd]);
                else
                {
                    os.validCommand = false; 
                    os.write("Failed to fetch manual for the command");
                }
            }
        }
    }
}
