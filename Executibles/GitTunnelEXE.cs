using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Hacknet;
using Hacknet.Gui;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Executibles
{
    public class GitTunnelEXE : ZeroDayEXE
    {
        public GitTunnelEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            Init(9418, 20f, 30f, 140, "GitTunnel For Hacknet BETA", "", "[GitTunnel For Hacknet BETA] Operation Finished");
        }
        public override void Update(float t)
        {
            if (!done && life >= runTime)
            {
                done = true;
                Computer c = Programs.getComputer(os, targetIP);
                c.openPort(port, os.thisComputer.ip);
                c.getFolderFromPath("log").files.Clear();
            }
            base.Update(t);
        }

        public override void Draw(float t)
        {
            base.Draw(t);
        }
    }
}
