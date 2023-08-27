using System;
using Hacknet;
using Pathfinder.Meta.Load;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;

namespace ZeroDayToolKit.TraceV2
{
    // load from save to os
    [SaveExecutor("HacknetSave.TraceV2", ParseOption.ParseInterior)]
    public class LoadTraceV2 : SaveLoader.SaveExecutor
    {
        public override void Execute(EventExecutor exec, ElementInfo info)
        {
            if (!info.Attributes.ContainsKey("name")) return;
            Network network = new Network();
            Network.networks[info.Attributes["name"]] = network;
            network.head = Programs.getComputer(Os, info.Attributes["head"]);
            network.tail.Add(network.head);
            string attr;
            foreach (ElementInfo child in info.Children)
            {
                switch (child.Name)
                {
                    case "trace":
                        network.traceTime = float.Parse(child.Attributes["time"]);
                        break;
                    case "reboot":
                        network.rebootTime = float.Parse(child.Attributes["time"]);
                        break;
                    case "Computer":
                        network.tail.Add(Programs.getComputer(Os, child.Attributes["name"]));
                        break;
                    case "onStart":
                        network.onStart = new NetworkTrigger();
                        network.onStart.action = child.Attributes["action"];
                        if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onStart.requireLogs = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onStart.sourceIntact = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("Delay", out attr)) network.onStart.delay = float.Parse(attr);
                        if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onStart.delayHost = attr;
                        break;
                    case "onCrash":
                        network.onCrash = new NetworkTrigger();
                        network.onCrash.action = child.Attributes["action"];
                        if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onCrash.requireLogs = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onCrash.sourceIntact = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("Delay", out attr)) network.onCrash.delay = float.Parse(attr);
                        if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onCrash.delayHost = attr;
                        break;
                    case "onComplete":
                        network.onComplete = new NetworkTrigger();
                        network.onComplete.action = child.Attributes["action"];
                        if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.onComplete.requireLogs = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.onComplete.sourceIntact = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("Delay", out attr)) network.onComplete.delay = float.Parse(attr);
                        if (child.Attributes.TryGetValue("DelayHost", out attr)) network.onComplete.delayHost = attr;
                        break;
                    case "afterComplete":
                        network.afterComplete = new AfterCompleteTrigger();
                        network.afterComplete.action = child.Attributes["action"];
                        if (child.Attributes.TryGetValue("RequireLogsOnSource", out attr)) network.afterComplete.requireLogs = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("RequireSourceIntact", out attr)) network.afterComplete.sourceIntact = bool.Parse(attr);
                        if (child.Attributes.TryGetValue("Delay", out attr)) network.afterComplete.delay = float.Parse(attr);
                        if (child.Attributes.TryGetValue("DelayHost", out attr)) network.afterComplete.delayHost = attr;
                        if (child.Attributes.TryGetValue("every", out attr)) network.afterComplete.every = int.Parse(attr);
                        if (child.Attributes.TryGetValue("offAfter", out attr)) network.afterComplete.offAfter = int.Parse(attr);
                        break;
                }
            }
        }
    }
}
