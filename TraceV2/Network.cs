using System.Collections.Generic;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.TraceV2
{
    public class Network
    {
        public static Dictionary<string, Network> networks = [];
        public static Dictionary<string, List<string>> postLoadComputerCache = [];
        public static List<AfterCompleteTrigger> afterCompleteTriggers = [];
        public static TraceV2Tracker tracker = new();
        public static Computer recentHostileActionTaken = null;
        public static Computer recentReboot = null;
        public static Computer recentCrash = null;
        public static Network recentRebootCompleted = null;
        public static int connections = 0;

        public Computer head;
        public List<Computer> tail = [];
        public float traceTime = -1;
        public float rebootTime = -1;
        public NetworkTrigger onStart;
        public NetworkTrigger onCrash;
        public NetworkTrigger onComplete;
        public AfterCompleteTrigger afterComplete;

        public static bool doesNetworkHaveLogsLeft(OS os, Network network)
        {
            foreach (Computer temp in network.tail) if (ComUtils.hasLogOnSource(os, temp)) return true;
            return false;
        }

        public static bool doesNetworkHaveSourceIntact(OS os, Network network)
        {
            foreach (Computer temp in network.tail) if (ComUtils.isSourceIntact(os, temp)) return true;
            return false;
        }
    }
}
