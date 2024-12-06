using System;
using System.Linq;
using System.Text.RegularExpressions;
using Hacknet;
using Hacknet.Daemons.Helpers;
using MonoMod.Cil;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Patches;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class SendIRC : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            IRCSystem irc = ComUtils.getIRC(c);
            if (irc == null)
            {
                os.write("This computer does not have an IRC Daemon.");
                os.validCommand = false;
                return;
            }
            var msg = string.Join(" ", args.Skip(1));
            if (LocaleTerms.ActiveTerms.ContainsKey("0dtk::allowed_messages")) {
                var loc = LocaleTerms.Loc("0dtk::allowed_messages");
                if (!SCOnIRCMessageAny.checkForWord(msg, loc))
                {
                    os.write("Chat filter is active, try the following list of words: " + string.Join(", ", loc.Split('|')));
                    os.validCommand = false;
                    return;
                }
            }
            string user = c.currentUser.name;
            msg = CheckSpecials(os, msg);
            SCOnIRCMessageAny.lastChatMessage = msg;
            user ??= os.SaveUserAccountName;
            irc.AddLog(user, msg);
        }

        public static string CheckSpecials(OS os, string msg)
        {
            var entry = ComUtils.GetPath(os, os.thisComputer, msg);
            if (entry.File != null)
            {
                if (new string[] { ".png", ".jpg", ".gif" }.Contains(ComUtils.getExtension(entry.File.data)))
                    return $"!ATTACHMENT:image#%#{entry.File.name}#%#{ImageFile.Binaries.Keys.First(x => ImageFile.Binaries[x] == entry.File.data)}";
                return $"!ATTACHMENT:file#%#{entry.File.name}#%#{entry.File.data}";
            }
            // legacy style
            if (new Regex(@"^\s*\[[^\]]+\]\([^\)]+\)\s*$").IsMatch(msg))
            {
                msg = msg.Substring(1, msg.Length - 2);
                var idx = msg.IndexOf("](");
                var left = msg.Substring(0, idx);
                var right = msg.Substring(idx + 2);
                if (left.Contains("@"))
                {
                    idx = left.IndexOf("@");
                    return $"!ATTACHMENT:account#%#{left.Substring(0, idx)}#%#{right}#%#{left.Substring(0, idx)}#%#{left.Substring(idx + 1)}";
                }
                return $"!ATTACHMENT:link#%#{left}#%#{right}";
            }
            // newstyle
            var m1 = new Regex(@"^\s*([^@]+?)\s*@\s*([^@]+?)\s*$").Match(msg);
            if (m1.Success) return $"!ATTACHMENT:link#%#{m1.Groups[1]}#%#{m1.Groups[2]}";
            var m2 = new Regex(@"^\s*([^@]+?)\s*@\s*([^@]+?)\s*@\s*([^@]+?)\s*$").Match(msg);
            if (m2.Success) return $"!ATTACHMENT:account#%#{m2.Groups[1]}#%#{m2.Groups[3]}#%#{m2.Groups[1]}#%#{m2.Groups[2]}";
            if (msg.StartsWith("!ATTACHMENT:") || msg.StartsWith("!ANNOUNCEMENT:")) return "***";
            return msg.Replace("\\n", "\n");
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(IRCSystem.IRCLogEntry), nameof(IRCSystem.IRCLogEntry.Serialize))]
    public class IRCNewLineSupport
    {
        public static string temp;
        public static void Prefix(ref IRCSystem.IRCLogEntry __instance)
        {
            temp = __instance.Message;
            __instance.Message = __instance.Message.Replace("\n", "\\n");
        }
        public static void Postfix(ref IRCSystem.IRCLogEntry __instance) { __instance.Message = temp; }
    }

    [HarmonyLib.HarmonyPatch(typeof(IRCSystem.IRCLogEntry), nameof(IRCSystem.IRCLogEntry.Deserialize))]
    public class IRCNewLineSupport2
    {
        public static string temp;
        public static void Postfix(ref IRCSystem.IRCLogEntry __result) { __result.Message = __result.Message.Replace("\\n", "\n"); }
    }
}
