using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCMessage : SCOnIRCMessageAny
    {
        [XMLStorage]
        public string word = "";
        [XMLStorage]
        public string Word = "";

        public override bool Filter(string msg)
        {
            return checkForWord(msg, word ?? Word);
        }
    }
}
