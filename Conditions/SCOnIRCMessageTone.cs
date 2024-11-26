using Hacknet;
using Hacknet.Localization;
using Pathfinder.Util;
using System.Linq;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCMessageTone : SCOnIRCMessageAny
    {
        [XMLStorage]
        public string tone = "";
        [XMLStorage]
        public string Tone = "";

        public override bool Check(object os_obj)
        {
            if (!base.Check(os_obj)) return false;
            OS os = (OS)os_obj;
            string msg = os.terminal.lastRunCommand;
            #region tone detection
            var t = tone ?? Tone;
            foreach (var term in LocaleTerms.ActiveTerms.Where(x => x.Key.StartsWith("0dtk::tone_")))
                if (t == term.Key.Substring("0dtk::tone_".Length)) return checkForWord(msg, term.Value);
            if (t == ZeroDayConditions.choice.ToString() && checkForWord(msg, "last|latter|bottom")) return true;
            if (t == ((ZeroDayConditions.choice + 1) / 2).ToString() && checkForWord(msg, "any|whatever|random|middle|center")) return true;
            #endregion
            return false;
        }
    }
}
