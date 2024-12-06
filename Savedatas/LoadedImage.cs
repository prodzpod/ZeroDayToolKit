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
    [SaveExecutor("HacknetSave.LoadedImage")]
    public class LoadedImage : SaveLoader.SaveExecutor
    {
        [Event]
        public static void Save(SaveEvent e)
        {
            foreach (var key in ImageFile.Binaries.Keys)
            {
                var el = new XElement("LoadedImage");
                el.SetAttributeValue("path", key);
                e.Save.Add(el);
            }
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            ImageFile.GetFile(info.Attributes["path"]);
        }
    }
}
