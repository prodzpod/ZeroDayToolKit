using Hacknet;
using Hacknet.Daemons.Helpers;
using Pathfinder.Port;

namespace ZeroDayToolKit.Utils
{
    public class ComUtils
    {
        public static Computer getComputer(OS os)
        {
            return os.connectedComp ?? os.thisComputer;
        }

        public static IRCSystem getIRC(Computer c)
        {
            IRCSystem irc = null;
            foreach (Daemon daemon in c.daemons)
            {
                if (daemon is DLCHubServer) irc = ((DLCHubServer)daemon).IRCSystem;
                else if (daemon is IRCDaemon) irc = ((IRCDaemon)daemon).System;
            }
            return irc;
        }

        public static bool isPortOpen(Computer c, int id)
        {
            return c.GetPortState(PortManager.GetPortRecordFromNumber(id).Protocol) != null;
        }
        public static bool hasLogOnSource(OS os, Computer c)
        {
            if (c == null) return false;
            if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(os, c, os.thisComputer.ip)) return true;
            return false;
        }
        public static bool isSourceIntact(OS os, Computer c)
        {
            if (c == null) return false;
            Folder sys = c.files.root.searchForFolder("sys");
            foreach (FileEntry file in sys.files) if (file.name == "netcfgx.dll" && file.data.Contains("0") && file.data.Contains("1")) return true;
            return false;
        }
    }
}
