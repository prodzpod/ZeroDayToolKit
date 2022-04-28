using System;
using Hacknet;

namespace ZeroDayToolKit.TraceV2
{


    public class NetworkTrigger
    {
        public OS os;
        public Network network;
        public Computer source;
        public string action;
        public float delay = 0f;
        public string delayHost;
        public bool requireLogs = false;
        public bool sourceIntact = false;
        public virtual void Start(OS os, Network network, Computer source)
        {
            this.os = os;
            this.network = network;
            this.source = source;
        }

        public void Trigger()
        {
            Console.WriteLine("Something Triggered");
            if (requireLogs && !Network.doesNetworkHaveLogsLeft(os, network)) return;
            if (sourceIntact && !Network.doesNetworkHaveSourceIntact(os, network)) return;
            SAAddConditionalActions action = new SAAddConditionalActions();
            action.Filepath = this.action;
            action.DelayHost = delayHost ?? source.idName;
            action.Delay = delay;
            DelayableActionSystem.FindDelayableActionSystemOnComputer(Programs.getComputer(os, delayHost)).AddAction(action, delay);
        }
    }
}
