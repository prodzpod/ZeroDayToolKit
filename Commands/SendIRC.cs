using System.Linq;
using Hacknet;
using Hacknet.Daemons.Helpers;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class SendIRC : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            IRCSystem irc = ComUtils.getIRC(c);
            if (irc == null)
            {
                os.write("This computer does not have an IRC Daemon.");
                return;
            }
            string user = c.currentUser.name;
            if (user == null) user = os.SaveUserAccountName;
            irc.AddLog(user, string.Join(" ", args.Skip(1)));

        }
    }
}
