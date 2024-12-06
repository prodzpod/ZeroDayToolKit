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
            if (string.IsNullOrWhiteSpace(word ?? Word)) return true;
            return checkForWord(msg, word ?? Word);
        }
    }
}
