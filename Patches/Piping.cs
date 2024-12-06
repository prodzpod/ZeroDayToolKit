using Microsoft.Xna.Framework;
using Hacknet;

using ZeroDayToolKit.TraceV2;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDayToolKit.Commands;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Utils;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.threadExecute))] // hook tracker update
    public class Piping
    {
        public static List<string> ConsumeAllCommands = ["alias", "echo", "expr", "send", "touch", "addnote", "append", "replace", "solve"];
        public static string LastCommandOutput = "";
        static bool Prefix(OS __instance, object threadText) // sorry for nuking but i didnt wanted to do this shit:tm: in ilmanip :v
        {
            try
            {
                string[] arguments = (string[])threadText;
                if (arguments.Length > 0 && arguments[0] == "save!(SJN!*SNL8vAewew57WewJdwl89(*4;;;&!)@&(ak'^&#@J3KH@!*") return true; // dont fw save
                if (!applyAliases(ref arguments))
                { __instance.write("Infinite loop detected. check your aliases."); return false; } // return now
                string temp = arguments.Join(" ");
                List<CommandArgument> commands = [];
                if (ConsumeAllCommands.Any(x => temp.ToLower() == x || temp.ToLower().StartsWith(x + " ")))
                {
                    commands = [new() {
                        args = temp.Split(' '),
                        appendResult = false,
                        requirement = (os, last) => true
                    }];
                }
                else
                {
                    CommandArgument now = new() { args = [], requirement = (os, last) => true, appendResult = false };
                    int prev = -1, next = temp.GoodIndexOf([" ", ";"], prev);
                    while (prev != temp.Length)
                    {
                        var endIsComma = next == temp.Length || temp[next] == ';';
                        var arg = temp.Slice(prev + 1, next).Trim();
                        // ZeroDayToolKit.Instance.Log.LogInfo("arg: " + arg);
                        if (now.args.Length == 0 && ConsumeAllCommands.Any(x => arg.ToLower() == x)) // end here
                        {
                            now.args = temp.Slice(prev + 1, temp.Length).Split(' ');
                            commands.Add(now);
                            break;
                        }
                        if (!string.IsNullOrWhiteSpace(arg))
                        {
                            if (arg == "&&" || arg == "||" || arg == "|" || arg == ">>")
                            {
                                // get all 
                                commands.Add(now);
                                // ZeroDayToolKit.Instance.Log.LogInfo($"submit: {now.args.Join(" ")} ({now.requirement}, {now.appendResult})");
                                now = new() { args = [] };
                                switch (arg)
                                {
                                    case "&&": now.requirement = (os, _) => os.validCommand; now.appendResult = false; break;
                                    case "||": now.requirement = (os, _) => !os.validCommand; now.appendResult = false; break;
                                    case "|": now.requirement = (os, _) => os.validCommand; now.appendResult = true; break;
                                    case ">>": now.args = ["touch"]; now.requirement = (os, last) => true; now.appendResult = true; break;
                                    default: now.requirement = (os, last) => true; now.appendResult = false; break;
                                }
                            }
                            else now.args = [.. now.args, arg];
                        }
                        if (endIsComma) 
                        { 
                            // ZeroDayToolKit.Instance.Log.LogInfo($"submit: {now.args.Join(" ")} ({now.requirement}, {now.appendResult})");
                            commands.Add(now); 
                            now = new() 
                            { 
                                args = [], 
                                requirement = (os, last) => true, 
                                appendResult = false 
                            }; 
                        }
                        prev = next;
                        next = temp.GoodIndexOf([" ", ";"], prev + 1);
                    }
                }
                commands = [.. commands.Where(x => x.args.Length > 0)];
                LastCommandOutput = "";
                foreach (var command in commands)
                {
                    if (!command.requirement(__instance, LastCommandOutput)) break;
                    var args = command.args;
                    if (command.appendResult) args = [.. args, .. LastCommandOutput.Trim().Split([' ', '\n'])];
                    LastCommandOutput = "";
                    __instance.validCommand = true; ProgramRunner.ExecuteProgram(__instance, args);
                    string[] words = args[0].Trim().ToLower() switch
                    {
                        "connect" => ["External Computer Refused Connection", "Failed to Connect:\nCould Not Find Computer"],
                        // disconnect: always true
                        "ls" => ["Insufficient Privileges to Perform Operation"],
                        "cd" => ["Insufficient Privileges to Perform Operation"],
                        "cat" => ["File Not Found", "Insufficient Privileges to Perform Operation"],
                        // exe: always true
                        // probe: always true
                        "scp" => ["Must be Connected to a Non-Local Host", "Not Enough Arguments", "Does Not Exist", "SCP ERROR: File not found.", "Insufficient Privileges to Perform Operation"],
                        "scan" => ["Scanning Requires Admin Access"],
                        "rm" => ["Not Enough Arguments", "Not found!", "Error - Insufficient Privileges"],
                        "mv" => ["Not Enough Arguments"],
                        // ps: true
                        "kill" => ["Error: Invalid PID or Input Format"],
                        "reboot" => ["Rebooting requires Admin access"],
                        "opencdtray" => ["Insufficient Privileges to Perform Operation"],
                        "closecdtray" => ["Insufficient Privileges to Perform Operation"],
                        "replace" => ["Not Enough Arguments", "not found.", "REPLACE ERROR: Replacement will cause file to be too long."],
                        "analyze" => ["No Firewall Detected"],
                        "solve" => ["Incorrect bypass sequence", "ERROR: No Solution provided", "No Firewall Detected"],
                        // clear: true
                        "upload" => ["Must be Connected to a Non-Local Host", "Not Enough Arguments", "Insufficient user permissions to upload", "not found.", "not found at specified filepath."],
                        "login" => ["Login Failed"],
                        // addnote: true
                        "append" => ["Usage: append [FILENAME] [LINE TO APPEND]", "File Not Found", "Insufficient Privileges to Perform Operation"],
                        "remline" => ["File Not Found", "Insufficient Privileges to Perform Operation"],
                        "addflag" => ["Flag to add required"],
                        "dscan" => ["Node ID Required", "Node ID Not found"],
                        "runcmd" => ["CAUTION: UNSYNDICATED OUTSIDE CONNECTION ATTEMPT"],
                        "runhackscript" => ["Error launching script"],
                        "help" => ["Invalid Page Number"],
                        _ => []
                    };
                    words = [.. words, "Fatal error has occured while executing this command, Command aborted.", "Execution Failed", "No Command"];
                    if (words.Any(x => LastCommandOutput.Contains(x))) __instance.validCommand = false;
                }
            }
            catch (Exception e) { ZeroDayToolKit.Instance.Log.LogError(e); }
            return false;
        }

        public struct CommandArgument
        {
            public string[] args;
            public Func<OS, string, bool> requirement;
            public bool appendResult;
        }

        public static bool applyAliases(ref string[] args)
        {
            string ret = args.Select(x => x.Trim()).Join(" ");
            if (string.IsNullOrWhiteSpace(ret)) return true;
            for (int i = 0; i < 127; i++)
            {
                bool replaced = false;
                foreach (var alias in ZeroDayConditions.aliases) if (Alias.FindAlias(ret, alias.Key) == 0)
                    {
                        var _args = ret.Split(' ');
                        ret = alias.Value;
                        for (int k = 0; k < Math.Min(10, _args.Length); k++) ret = ret.Replace("$" + k, _args[k]);
                        ret = ret.Replace("$#", _args.Length.ToString()).Replace("$*", _args.Range(1).Join(" ")).Trim();
                        replaced = true;
                    }
                if (!replaced) { args = ret.Split(' '); return true; }
            }
            return false;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.write))]
    public class HijackOSWrite { public static void Postfix(string text) { Piping.LastCommandOutput += text + "\n"; } }

    [HarmonyLib.HarmonyPatch(typeof(OS), nameof(OS.writeSingle))]
    public class HijackOSWrite2 { public static void Postfix(string text) { Piping.LastCommandOutput += text; } }
}
