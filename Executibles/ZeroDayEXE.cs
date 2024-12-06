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
            int status = -1;
            if (noProxy && c.proxyActive) status = 0;
            else if (Args.Length <= 1) status = 1;
            else if (!int.TryParse(Args[1], out _)) status = 2;
            else if (int.Parse(Args[1]) != port) status = ComUtils.isPortOpen(c, c.GetCodePortNumberFromDisplayPort(int.Parse(Args[1]))) ? 3 : 2;
            else if (!ComUtils.isPortOpen(c, originPort)) status = 2;
            if (status != -1) 
            {
                os.write(new string[] {"Proxy Active", "No port number Provided", "Target Port is Closed", "Target Port running incompatible service for this executable"}[status]);
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
                Completed();
                foreach (string line in endMessage.Split('\n')) os.write(line);
            }
            else if (!isExiting && life >= (runTime + exitTime)) isExiting = true;
            incrementLife(t);
            base.Update(t);
        }

        public override void Completed()
        {
            base.Completed();
            Programs.getComputer(os, targetIP).openPort(port, os.thisComputer.ip);
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
