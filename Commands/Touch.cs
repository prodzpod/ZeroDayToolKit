using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Touch : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (!os.hasConnectionPermission(true)) { os.write("Insufficient Privileges to Perform Operation"); os.validCommand = false; }
            else if (args.Length < 2) { os.write("Usage: touch [filename] [OPTIONAL: contnent]"); os.validCommand = false; }
            else ComUtils.getComputer(os).makeFile(os.thisComputer.ip, ComUtils.getNoDupeFileName(args[1], os), args.Length < 3 ? "" : string.Join(" ", args.Skip(2)), os.navigationPath);
        }
    }
}
