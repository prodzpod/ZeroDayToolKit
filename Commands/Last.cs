using Hacknet;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Last : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            os.write("Latest connection logs:");
            foreach (var log in c.files.root.searchForFolder("log").files)
            {
                if (!log.name.StartsWith("@") || !log.name.Contains("_Connection:_")) continue;
                os.write(log.data);
            }
        }
    }
}
