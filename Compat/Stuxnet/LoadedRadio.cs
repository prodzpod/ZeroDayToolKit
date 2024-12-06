using Pathfinder.Event.Saving;
using Pathfinder.Meta.Load;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;
using System.Xml.Linq;

namespace ZeroDayToolKit.Compat.Stuxnet
{
    [SaveExecutor("HacknetSave.LoadedRadio")]
    public class LoadedRadio : SaveLoader.SaveExecutor
    {
        // if stuxnet is not real, should not save or load anything as StuxnetCompat.RadioBinaries is empty;
        [Event]
        public static void Save(SaveEvent e)
        {
            foreach (var key in StuxnetCompat.RadioBinaries.Keys)
            {
                var el = new XElement("LoadedRadio");
                el.SetAttributeValue("id", key);
                e.Save.Add(el);
            }
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            StuxnetCompat.GetRadio(info.Attributes["id"]);
        }
    }
}
