using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroDayToolKit.Conditions
{
    public abstract class ZeroDayConditions
    {
        public static Dictionary<string, TimeSpan> times = [];
        public static int choice = 3;
        public static List<string> defaultDisabledCommands = [
            "send",
            "date",
            "source",
            "grep",
            "host",
            "updatedb",
            "find",
            "locate",
            "alias",
            "unalias",
            "rmdir",
            "diff"
        ];
        public static List<string> disabledCommands = [.. defaultDisabledCommands];
        public static Dictionary<string, string> defaultAliases = new() { // dont modify this
            // vanilla: added to aliases so disablecommand works for all alias
            { "dc", "disconnect $*" },
            { "dir", "ls $*" },
            { "more", "cat $*" },
            { "less", "cat $*" },
            { "nmap", "probe $*" },
            { "del", "rm $*" },
            { "pkill", "kill $*" },
            { "up", "upload $*" },
            { ":(){:|:&};:", "forkbomb" },
            // 0dtk
            { "/", "send $*" },
            { ">", "send $*" },
            { "irc", "send $*" },
            { "whoami", "who" },
            { "w", "last" },
            { "shutdown now", "reboot -i" },
            { "ip addr show", "scan" },
            { "ip address show", "scan" },
            { "ip addr add", "connect $3" },
            { "ip address add", "connect $3" },
            { "netstat", "probe" },
        };
        public static Dictionary<string, string> aliases = new(defaultAliases);
    }
}
