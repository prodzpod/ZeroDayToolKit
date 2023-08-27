using Microsoft.Xna.Framework;
using Hacknet;
using Pathfinder.Util;
using Pathfinder.Executable;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Executibles
{
    public abstract class ZeroDayEXE : BaseExecutable
    {
        [System.Obsolete]
        public override string GetIdentifier() => IdentifierName;
        public int originPort = 0;
        public int port;
        public string startMessage = "";
        public string endMessage = "";
        public bool noProxy = true;
        public bool noHostile = false;
        public float life = 0f;
        public float runTime = 5f;
        public float exitTime = 0f;
        public bool done = false;
        public ZeroDayEXE(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args) { }
        public void Init(int port, float runTime, int ramCost, string id, string startMessage, string endMessage) { Init(port, runTime, 0f, ramCost, id, startMessage, endMessage); }
        public void Init(int port, float runTime, float exitTime, int ramCost, string id, string startMessage, string endMessage)
        {
            originPort = port;
            this.runTime = runTime;
            this.exitTime = exitTime;
            this.ramCost = ramCost;
            IdentifierName = id;
            this.startMessage = startMessage;
            this.endMessage = endMessage;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Computer c = ComputerLookup.FindByIp(targetIP);
            port = c.GetDisplayPortNumberFromCodePort(originPort);
            if (noProxy && c.proxyActive)
            {
                os.write("Proxy Active");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else if (Args.Length <= 1)
            {
                os.write("No port number Provided");
                os.write("Execution failed");
                needsRemoval = true;
            }
            else if (!int.TryParse(Args[1], out _) || int.Parse(Args[1]) != port || !ComUtils.isPortOpen(c, originPort))
            {
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
                done = true;
                Programs.getComputer(os, targetIP).openPort(port, os.thisComputer.ip);
                foreach (string line in endMessage.Split('\n')) os.write(line);
            }
            else if (!isExiting && life >= (runTime + exitTime)) isExiting = true;
            incrementLife(t);
            base.Update(t);
        }

        public virtual void incrementLife(float t)
        {
            life += t;
        }
        public override void Draw(float t)
        {
            base.Draw(t); drawTarget(); drawOutline();
        }
        public void drawLine(Vector2 origin, Vector2 dest, Color c)
        {
            DrawUtils.drawLine(spriteBatch, origin, dest, c);
        }
    }
}
