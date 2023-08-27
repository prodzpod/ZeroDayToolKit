using Hacknet;
using Pathfinder.Event.Saving;
using Pathfinder.Meta.Load;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Patches;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Savedatas
{
    [SaveExecutor("HacknetSave.NetworkMap.network.computer.StrictLogEnabled")]
    public class StrictLogs : SaveLoader.SaveExecutor
    {
        [Event]
        public static void Save(SaveEvent e)
        {
            foreach (var c in TrackerCheckLogs.stricts)
            {
                var el = SaveUtils.Path(e, "HacknetSave.NetworkMap.network.computer", x => x.Attribute("id").Value == c.idName);
                el.SetAttributeValue("StrictLogEnabled", true);
            }
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            TrackerCheckLogs.stricts.Add(Programs.getComputer(Os, info.Parent.Attributes["id"]));
        }
    }
}
