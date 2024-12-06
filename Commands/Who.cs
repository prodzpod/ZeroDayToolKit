using Hacknet;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Who : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            var type = -1;
            if (c.adminIP == os.thisComputer.ip || (c.currentUser.type == 0 && c.currentUser.name != null)) type = 0;
            else if (c != null && c.userLoggedIn) type = 1;
            switch (type)
            {
                case -1: os.write($"Active Accounts in {c.ip}: "); break;
                case 0: os.write($"Active Accounts in {c.ip}: {os.SaveUserAccountName} (ADMIN)"); break;
                case 1: os.write($"Active Accounts in {c.ip}: {os.SaveUserAccountName}"); break;
            }
        }
    }
}
