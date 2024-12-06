using Pathfinder.Event.Saving;
using Pathfinder.Meta.Load;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;
using System.Linq;
using System.Xml.Linq;
using ZeroDayToolKit.Patches;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Savedatas
{
    [SaveExecutor("HacknetSave.RAMAmount")]
    public class RAMAmount : SaveLoader.SaveExecutor
    {
        [Event]
        public static void Save(SaveEvent e)
        {
            var el = new XElement("RAMAmount");
            el.SetAttributeValue("ram", e.Os.totalRam);
            e.Save.Add(el);
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            Os.totalRam = int.Parse(info.Attributes["ram"]);
        }
    }
}
