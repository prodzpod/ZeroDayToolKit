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

        public static string getNoDupeFileName(string name, OS os)
        {
            Folder folder = Programs.getCurrentFolder(os);
            if (!folder.containsFile(name) && folder.searchForFolder(name) == null) return name;
            for (int i = 1;; i++)
            {
                string newName = name + "(" + i + ")";
                if (!folder.containsFile(newName) && folder.searchForFolder(newName) == null) return newName;
            }
        }

        public static string getExtension(string data)
        {
            if (PortExploits.crackExeData.ContainsValue(data)) return ".exe";
            if (PortExploits.crackExeDataLocalRNG.ContainsValue(data)) return ".exe";
            // detect custom exe
            if (ThemeManager.fileData.ContainsValue(data)) return ".sys";
            if (data.Contains("<GitCommitEntry>")) return ".rec";
            if (data.Contains("##DHS_CONFIG")) return ".sys";
            if (data.Contains("RequireAuth:")) return ".cfg";
            if (data.EndsWith("--------------------")) return ".tm";
            if (data.Contains("<html>")) return ".html";
            if (data.StartsWith("@")) return "";
            if (data.Contains("MEMORY_DUMP : FORMAT")) return ".md";
            if (data.Contains("Archived Via : http://Bash.org")) return "";
            if (data.Contains("#DEC_ENC")) return ".dec";
            if (data.EndsWith("\n//")) return ".zip";
            if (MathUtils.decodeBinary(data) != null) return ".bin";
            return ".txt";
        }
    }
}
