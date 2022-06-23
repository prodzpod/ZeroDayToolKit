using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Touch : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            ComUtils.getComputer(os).makeFile(os.thisComputer.ip, ComUtils.getNoDupeFileName(args[1], os), args.Length < 2 ? "" : string.Join(" ", args.Skip(1)), os.navigationPath);
        }
    }
}
