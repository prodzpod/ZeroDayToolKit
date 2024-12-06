using Hacknet;
using System.Threading;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Sleep : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2 || !float.TryParse(args[1], out float s)) { os.validCommand = false; os.write("Usage: sleep [seconds]"); return; }
            Thread.Sleep((int)(s * 1000));
        }
    }
}
