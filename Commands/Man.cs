using Hacknet;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Patches;

namespace ZeroDayToolKit.Commands
{
    public class Man : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: man [command]"); }
            else
            {
                var cmd = args[1].ToLower(); var _cmd = cmd;
                if (ZeroDayConditions.defaultAliases.ContainsKey(cmd)) cmd = ZeroDayConditions.defaultAliases[cmd];
                if (BetterHelp.Descriptions.ContainsKey(cmd)) os.write("Usage: " + _cmd + " " + BetterHelp.Usages[cmd] + "\n  " + BetterHelp.Descriptions[cmd]);
                else
                {
                    os.validCommand = false; 
                    os.write("Failed to fetch manual for the command");
                }
            }
        }
    }
}
