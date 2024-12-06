using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCAttachment : SCOnIRCMessageAny
    {
        [XMLStorage]
        public string name = "";
        [XMLStorage]
        public string Name = "";

        public override bool Filter(string msg)
        {
            if (!msg.StartsWith("!ATTACHMENT:")) return false;
            return Filter2(msg.Substring("!ATTACHMENT:".Length).Split(["#%#"], System.StringSplitOptions.None));
        }

        public virtual bool Filter2(string[] attachment)
        {
            if (string.IsNullOrWhiteSpace(name ?? Name)) return true;
            return attachment[1] == (name ?? Name);
        }
    }
}
