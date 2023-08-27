using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class MakeDir : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (!os.hasConnectionPermission(true)) { os.validCommand = false; os.write("Insufficient Privileges to Perform Operation"); }
            else if (args.Length < 2) { os.validCommand = false; os.write("Usage: mkdir [foldername]"); }
            else ComUtils.getComputer(os).makeFolder(os.thisComputer.ip, ComUtils.getNoDupeFileName(args[1], os), os.navigationPath);
        }
    }
}
