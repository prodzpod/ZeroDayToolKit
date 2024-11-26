using System.Linq;
using Hacknet;
using Hacknet.Daemons.Helpers;
using ZeroDayToolKit.Conditions;
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
                os.validCommand = false;
                return;
            }
            var msg = string.Join(" ", args.Skip(1));
            if (LocaleTerms.ActiveTerms.ContainsKey("0dtk::allowed_messages")) {
                var loc = LocaleTerms.Loc("0dtk::allowed_messages");
                if (!SCOnIRCMessageAny.checkForWord(msg, loc))
                {
                    os.write("Chat filter is active, try the following list of words: " + string.Join(", ", loc.Split('|')));
                    os.validCommand = false;
                    return;
                }
            }
            string user = c.currentUser.name;
            if (user == null) user = os.SaveUserAccountName;
            irc.AddLog(user, msg);
        }
    }
}
