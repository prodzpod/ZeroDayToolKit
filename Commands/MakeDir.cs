using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class MakeDir : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            ComUtils.getComputer(os).makeFolder(os.thisComputer.ip, ComUtils.getNoDupeFileName(args[1], os), os.navigationPath);
        }
    }
}
