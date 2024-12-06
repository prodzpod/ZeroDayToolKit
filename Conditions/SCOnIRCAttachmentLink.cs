using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCAttachmentLink : SCOnIRCAttachment
    {
        [XMLStorage]
        public string ip = "";
        [XMLStorage]
        public string Ip = "";
        [XMLStorage]
        public string IP = "";

        public override bool Filter2(string[] attachment)
        { 
            if (attachment[0] != "link"
                || (!string.IsNullOrWhiteSpace(ip ?? Ip ?? IP) && attachment[2] != (ip ?? Ip ?? IP))) return false;
            return base.Filter2(attachment);
        }
    }
}
