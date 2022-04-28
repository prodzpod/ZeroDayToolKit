using System;
using System.Collections.Generic;
using Hacknet;
using Hacknet.Daemons.Helpers;
using Pathfinder.Util;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnIRCMessageAny : Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public string target = null;
        [XMLStorage]
        public string user = null;
        [XMLStorage]
        public string notUser = null;
        [XMLStorage]
        public float minDelay = 0.0f;
        [XMLStorage]
        public float maxDelay = float.MaxValue;
        [XMLStorage]
        public string requiredFlags;
        [XMLStorage]
        public string doesNotHaveFlags;

        public override bool Check(object os_obj)
        {
            OS os = (OS)os_obj;
            Computer c = ComUtils.getComputer(os);
            if (!string.IsNullOrWhiteSpace(requiredFlags)) foreach (string flag in requiredFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (!os.Flags.HasFlag(flag)) return false;
            if (!string.IsNullOrWhiteSpace(doesNotHaveFlags)) foreach (string flag in doesNotHaveFlags.Split(Hacknet.Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries)) if (os.Flags.HasFlag(flag)) return false;
            if (target != null && c != Programs.getComputer(os, target)) return false;
            if (user == "#PLAYERNAME#" && (c.currentUser.name != null && c.currentUser.name != os.SaveUserAccountName)) return false;
            if (!string.IsNullOrWhiteSpace(user) && c.currentUser.name != user) return false;
            if (notUser == "#PLAYERNAME#" && (c.currentUser.name == null || c.currentUser.name == os.SaveUserAccountName)) return false;
            if (!string.IsNullOrWhiteSpace(notUser) && c.currentUser.name == notUser) return false;
            if (ZeroDayCondition.times.ContainsKey(target))
            {
                float delay = (float)(os.lastGameTime.TotalGameTime - ZeroDayCondition.times[target]).TotalSeconds;
                if (minDelay > delay || delay > maxDelay) return false;
            }
            IRCSystem irc = ComUtils.getIRC(c);
            if (irc == null) return false;
            string[] args = os.terminal.lastRunCommand.Split(Hacknet.Utils.WhitespaceDelim, StringSplitOptions.RemoveEmptyEntries);
            return args[0] == ">";
        }

        public static bool checkForWord(string msg, string parse)
        {
            string[] temp = parse.Split('&');
            List<string>[] words = new List<string>[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                words[i] = new List<string>(temp[i].Split('|'));
                bool found = false;
                foreach (string w in words[i]) if (msg.ToLower().Contains(w.ToLower().Trim())) found = true;
                if (!found) return false;
            }
            return true;
        }
    }
}
