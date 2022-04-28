using System;
using Microsoft.Xna.Framework;
using Hacknet;
using Hacknet.Gui;
using Pathfinder.Util;
using Pathfinder.Port;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Executibles
{
    public class PortBackdoorEXE : ZeroDayEXE
    {
        public PortBackdoorEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(0, 40f, 5f, 400, "thanks from /el/ <3", "TOP TEXT", "BOTTOM TEXT");
        }

        public override void LoadContent()
        {
            Computer c = ComputerLookup.FindByIp(targetIP);
            port = c.GetDisplayPortNumberFromCodePort(originPort);
            if (noProxy && c.proxyActive)
            {
                os.write("Proxy Active");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else if ((Args.Length > 1 && (!int.TryParse(Args[1], out _) || int.Parse(Args[1]) != port)) || (Args.Length <= 1 && port != 0))
            { // if no args given, use 65535
                os.write("Target Port is Closed");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else foreach (string line in startMessage.Split('\n')) os.write(line);
            if (!noHostile) Programs.getComputer(os, targetIP).hostileActionTaken();
        }
        public override void Update(float t)
        {
            if (!done && life >= runTime)
            {
                Computer c = Programs.getComputer(os, targetIP);
                done = true;
                foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                Console.WriteLine(ComUtils.isPortOpen(c, originPort));
                if (!ComUtils.isPortOpen(c, originPort)) // now it has port 0
                    c.AddPort(PortManager.GetPortRecordFromNumber(originPort).CreateState(c, true));
                c.openPort(port, os.thisComputer.ip);
                foreach (PortState s in c.GetAllPortStates()) Console.WriteLine(s.Record.DefaultPortNumber);
                foreach (string line in endMessage.Split('\n')) os.write(line);
            }
            base.Update(t);
        }

        public override void Draw(float t)
        {
            TextItem.doCenteredFontLabel(Bounds, "test graphic", GuiData.detailfont, Color.White * fade);
            base.Draw(t);
        }
    }
}
