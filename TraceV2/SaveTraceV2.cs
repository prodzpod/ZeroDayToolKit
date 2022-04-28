using System.Linq;
using System.Xml.Linq;
using Pathfinder.Meta.Load;
using Pathfinder.Event.Saving;

namespace ZeroDayToolKit.TraceV2
{
    public static class SaveTraceV2
    {
        [Event] // save from os to save
        public static void SaveNetworkHandler(SaveEvent e)
        {
            foreach (string key in Network.networks.Keys)
            {
                Network value = Network.networks[key];
                XElement network = new XElement("TraceV2");
                network.SetAttributeValue("name", key);
                network.SetAttributeValue("head", value.tail[0].idName);
                XElement trace = new XElement("trace");
                trace.SetAttributeValue("time", value.traceTime);
                network.Add(trace);
                XElement reboot = new XElement("reboot");
                reboot.SetAttributeValue("time", value.rebootTime);
                network.Add(reboot);
                if (value.onStart != null)
                {
                    XElement onStart = new XElement("onStart");
                    onStart.SetAttributeValue("action", value.onStart.action);
                    if (value.onStart.requireLogs) onStart.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onStart.sourceIntact) onStart.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onStart.delay != 0f) onStart.SetAttributeValue("Delay", value.onStart.delay);
                    if (value.onStart.delayHost != null) onStart.SetAttributeValue("DelayHost", value.onStart.delayHost);
                    network.Add(onStart);
                }
                if (value.onCrash != null)
                {
                    XElement onCrash = new XElement("onCrash");
                    onCrash.SetAttributeValue("action", value.onCrash.action);
                    if (value.onCrash.requireLogs) onCrash.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onCrash.sourceIntact) onCrash.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onCrash.delay != 0f) onCrash.SetAttributeValue("Delay", value.onCrash.delay);
                    if (value.onCrash.delayHost != null) onCrash.SetAttributeValue("DelayHost", value.onCrash.delayHost);
                    network.Add(onCrash);
                }
                if (value.onComplete != null)
                {
                    XElement onComplete = new XElement("onComplete");
                    onComplete.SetAttributeValue("action", value.onComplete.action);
                    if (value.onComplete.requireLogs) onComplete.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.onComplete.sourceIntact) onComplete.SetAttributeValue("RequireSourceIntact", true);
                    if (value.onComplete.delay != 0f) onComplete.SetAttributeValue("Delay", value.onComplete.delay);
                    if (value.onComplete.delayHost != null) onComplete.SetAttributeValue("DelayHost", value.onComplete.delayHost);
                    network.Add(onComplete);
                }
                if (value.afterComplete != null)
                {
                    XElement afterComplete = new XElement("afterComplete");
                    afterComplete.SetAttributeValue("action", value.afterComplete.action);
                    if (value.afterComplete.requireLogs) afterComplete.SetAttributeValue("RequireLogsOnSource", true);
                    if (value.afterComplete.sourceIntact) afterComplete.SetAttributeValue("RequireSourceIntact", true);
                    if (value.afterComplete.delay != 0f) afterComplete.SetAttributeValue("Delay", value.afterComplete.delay);
                    if (value.afterComplete.delayHost != null) afterComplete.SetAttributeValue("DelayHost", value.afterComplete.delayHost);
                    if (value.afterComplete.every != 1) afterComplete.SetAttributeValue("every", value.afterComplete.every);
                    if (value.afterComplete.offAfter != -1) afterComplete.SetAttributeValue("offAfter", value.afterComplete.offAfter);
                    network.Add(afterComplete);
                }
                for (int i = 1; i < value.tail.Count; i++)
                {
                    XElement comp = new XElement("Computer");
                    comp.SetAttributeValue("name", value.tail[i].idName);
                    network.Add(comp);
                }
                e.Save.Add(network);
            }
        }
    }
}
