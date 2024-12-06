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
    [SaveExecutor("HacknetSave.CommandAlias")]
    public class CommandAliases : SaveLoader.SaveExecutor
    {
        [Event]
        public static void Save(SaveEvent e)
        {
            foreach (var alias in ZeroDayConditions.aliases)
            {
                var el = new XElement("CommandAlias");
                el.SetAttributeValue("command", alias.Key);
                el.SetAttributeValue("expression", alias.Value);
                e.Save.Add(el);
            }
        }
        public override void Execute(EventExecutor exec, ElementInfo info) { Load(info); }
        public void Load(ElementInfo info)
        {
            ZeroDayConditions.aliases[info.Attributes["command"]] = info.Attributes["expression"];        
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Hacknet.PlatformAPI.Storage.SaveFileManager), nameof(Hacknet.PlatformAPI.Storage.SaveFileManager.GetSaveReadStream))]
    public class InitializeDisabledCommands
    {
        public static void Prefix()
        {
            ZeroDayConditions.disabledCommands.Clear();
            ZeroDayConditions.aliases.Clear();
        }
    }
}
