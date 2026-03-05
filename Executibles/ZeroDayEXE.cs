using Microsoft.Xna.Framework;
using Hacknet;
using Pathfinder.Util;
using Pathfinder.Executable;

using ZeroDayToolKit.Utils;
using System;

namespace ZeroDayToolKit.Executibles
{
    public abstract class ZeroDayEXE : GameExecutable
    {
        public int originPort = 0;
        public int port;
        public string startMessage = "";
        public string endMessage = "";
        public bool isHostileAction = true;
        public float runTime = 5f;
        public float exitTime = 0f;
        public ZeroDayEXE(): base() {}
        public bool done = false;
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

        public override void OnInitialize()
        {
            base.OnInitialize();
            Computer c = ComputerLookup.FindByIp(targetIP);
            port = c.GetDisplayPortNumberFromCodePort(originPort);
            int status = 0;
            if (Args.Length <= 1) status = 1;
            else if (!int.TryParse(Args[1], out _)) status = 2;
            else if (int.Parse(Args[1]) != port) status = ComUtils.isPortOpen(c, c.GetCodePortNumberFromDisplayPort(int.Parse(Args[1]))) ? 3 : 2;
            else if (!ComUtils.isPortOpen(c, originPort)) status = 2;
            if (status != 0) 
            {
                os.write(new string[] {"No port number Provided", "Target Port is Closed", "Target Port running incompatible service for this executable"}[status - 1]);
                os.write("Execution failed");
                needsRemoval = true;
            }
            else foreach (string line in startMessage.Split('\n')) os.write(line);
            if (isHostileAction) Programs.getComputer(os, targetIP).hostileActionTaken();
        }
        public override void OnProxyBypassFailure() { base.OnProxyBypassFailure(); needsRemoval = true; }
        public override void OnNoAvailableRam() { base.OnNoAvailableRam(); needsRemoval = true; }
        public override void Update(float t)    
        {
            Lifetime -= t; IncrementLife(t);
            base.Update(t);
        }
        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            if (!done && Lifetime >= runTime)
            {
                done = true;
                OnSuccess();
            }
            else if (!isExiting && Lifetime >= (runTime + exitTime)) Result = CompletionResult.Success;
        }

        public virtual void OnSuccess()
        {
            var c = Programs.getComputer(os, targetIP);
            Console.Write(c.ports.Join(", "));
            Console.Write(c.PortRemapping.Keys.Join(", "));
            Console.Write(c.PortRemapping.Values.Join(", "));
            Console.Write(port);
            c.openPort(port, os.thisComputer.ip);
            foreach (string line in endMessage.Split('\n')) os.write(line);
        }

        public virtual void IncrementLife(float t) { Lifetime += t; }
        public override void Draw(float t)
        {
            base.Draw(t); drawOutline(); drawTarget();
        }
        public void drawLine(Vector2 origin, Vector2 dest, Color c)
        {
            DrawUtils.drawLine(spriteBatch, origin, dest, c);
        }
    }
}
