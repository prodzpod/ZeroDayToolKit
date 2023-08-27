using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Echo : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) os.write("");
            else os.write(string.Join(" ", args.Skip(1)));
        }
    }
}
