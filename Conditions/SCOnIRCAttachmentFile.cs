using Hacknet;
using Pathfinder.Util;
using System.Linq;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCAttachmentFile : SCOnIRCAttachment
    {
        [XMLStorage]
        public string content = "";
        [XMLStorage]
        public string Content = "";

        public override bool Filter2(string[] attachment)
        {
            if (!new string[] { "file", "image", "radio" }.Contains(attachment[0])
                || (!string.IsNullOrWhiteSpace(content ?? Content) && ComputerLoader.filter(attachment[2]) != (content ?? Content))) return false;
            return base.Filter2(attachment);
        }
    }
}
