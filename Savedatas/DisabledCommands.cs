using Pathfinder.Event.Saving;
using Pathfinder.Meta.Load;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;
using System.Xml.Linq;
using ZeroDayToolKit.Conditions;

namespace ZeroDayToolKit.Savedatas
{
    [SaveExecutor("HacknetSave.DisabledCommand")]
    public class DisabledCommands : SaveLoader.SaveExecutor
    {
        [Event]
        public static void Save(SaveEvent e)
        {
            foreach (var command in ZeroDayConditions.disabledCommands)
            {
                var el = new XElement("DisabledCommand");
                el.SetAttributeValue("command", command);
                e.Save.Add(el);
            }
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            ZeroDayConditions.disabledCommands.Add(info.Attributes["command"]);
        }
    }
}
