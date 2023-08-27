using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class History : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            os.write(string.Join("\n", os.terminal.runCommands));
        }
    }
}
