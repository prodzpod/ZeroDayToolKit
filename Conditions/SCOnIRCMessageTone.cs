using Hacknet;
using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCMessageTone : SCOnIRCMessageAny
    {
        [XMLStorage]
        public string tone = "";

        public override bool Check(object os_obj)
        {
            if (!base.Check(os_obj)) return false;
            OS os = (OS)os_obj;
            string msg = os.terminal.lastRunCommand;
            #region tone detection
            if (tone == "no" && checkForWord(msg, "no|na|eh|sorry|can't|cant|decline|no can|off|won't|wont|couldn't|couldnt|shouldn't|shouldnt|shant|shan't")) return true; // early exit so "no can do" is "no"
            if (tone == "yes" && checkForWord(msg, "ye|yup|ok|alr|aight|lets|les|let's|leg|sure|wish me|got it|got this|cool|ready|rdy|can do|will|accept|bring|here i|here we|could|would|should|ought|shall")) return true;
            if (tone == "help" && checkForWord(msg, "stuck|can't|cant|what|hm|huh|?|not|help|aid|idea|how|hint|clue|doesn|nudge")) return true;
            if (tone == "hey" && checkForWord(msg, "guy|boy|girl|dude|folk|people|@channel|yall|hey|sup")) return true;
            if (tone == "1" && checkForWord(msg, "1|one|first|former")) return true;
            if (tone == "2" && checkForWord(msg, "2|two|second")) return true;
            if (tone == "3" && checkForWord(msg, "3|three|third")) return true;
            if (tone == "4" && checkForWord(msg, "4|four|fourth")) return true;
            if (tone == "5" && checkForWord(msg, "5|five|fifth")) return true;
            if (tone == "6" && checkForWord(msg, "6|six|sixth")) return true;
            if (tone == "7" && checkForWord(msg, "7|seven|seventh")) return true;
            if (tone == "8" && checkForWord(msg, "8|eight|eighth")) return true;
            if (tone == "9" && checkForWord(msg, "9|nine|ninth")) return true;
            if (tone == "10" && checkForWord(msg, "10|ten|tenth")) return true;
            if (tone == ZeroDayConditions.choice.ToString() && checkForWord(msg, "last|latter|bottom")) return true;
            if (tone == ((ZeroDayConditions.choice + 1) / 2).ToString() && checkForWord(msg, "any|whatever|random|middle|center")) return true;
            #endregion
            return false;
        }
    }
}
