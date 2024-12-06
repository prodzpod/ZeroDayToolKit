using Hacknet;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Hostname : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            var c = ComUtils.getComputer(os);
            if (args.Length > 1 && args[1] == "-i") os.write(c.ip);
            else os.write(c.name);
        }
    }
}
