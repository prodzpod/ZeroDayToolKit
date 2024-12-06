using Pathfinder.Util;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCAttachmentAccount : SCOnIRCAttachment
    {
        [XMLStorage]
        public string ip = "";
        [XMLStorage]
        public string Ip = "";
        [XMLStorage]
        public string IP = "";
        [XMLStorage]
        public string id = "";
        [XMLStorage]
        public string Id = "";
        [XMLStorage]
        public string ID = "";
        [XMLStorage]
        public string password = "";
        [XMLStorage]
        public string Password = "";

        public override bool Filter2(string[] attachment)
        { 
            if (attachment[0] != "account"
                || (!string.IsNullOrWhiteSpace(ip ?? Ip ?? IP) && attachment[2] != (ip ?? Ip ?? IP))
                || (!string.IsNullOrWhiteSpace(id ?? Id ?? ID) && attachment[3] != (id ?? Id ?? ID)) 
                || (!string.IsNullOrWhiteSpace(password ?? Password) && attachment[4] != (password ?? Password))) return false;
            return base.Filter2(attachment);
        }
    }
}
