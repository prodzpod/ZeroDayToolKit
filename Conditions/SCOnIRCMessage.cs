using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCMessage : SCOnIRCMessageAny
    {
        [XMLStorage]
        public string word = "";

        public override bool Check(object os_obj)
        {
            if (!base.Check(os_obj)) return false;
            OS os = (OS)os_obj;
            return checkForWord(os.terminal.lastRunCommand, word);
        }
    }
}
