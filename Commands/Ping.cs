using Hacknet;
using System.Threading;
using System;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Patches;

namespace ZeroDayToolKit.Commands
{
    public class Ping : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: ping [ip]"); return; }
            os.write("Pinging " + args[1]);
            os.write(".");
            for (int i = 0; i < 3; i++)
            {
                os.writeSingle(".");
                Thread.Sleep(200);
            }
            foreach (var node in os.netMap.nodes)
            {
                if (node.ip != args[1] && node.name != args[1]) continue;
                if (node.connect(os.thisComputer.ip))
                {
                    var i = os.netMap.nodes.IndexOf(node);
                    if (!os.netMap.visibleNodes.Contains(i)) os.netMap.visibleNodes.Add(i);
                    os.write("Found Computer at " + args[1]);
                    return;
                }
                else { os.validCommand = false; os.write("External Computer Refused Connection"); return; }
            }
            os.validCommand = false; os.write("Failed to Connect:\nCould Not Find Computer at " + args[1]);
            return;
        }
    }
}
