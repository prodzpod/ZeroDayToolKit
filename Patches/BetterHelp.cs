using Hacknet;
using System.Collections.Generic;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.Locales;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Helpfile), nameof(Helpfile.init))] 
    public class BetterHelp
    {
        public static Dictionary<string, string> Usages = new Dictionary<string, string>();
        public static Dictionary<string, string> Descriptions = new Dictionary<string, string>();

        public static void AddVanilla()
        {
            Add("help", "[PAGE NUMBER]", "Displays the specified page of commands.");
            Add("scp", "[filename] [OPTIONAL: destination]", "Copies file named [filename] from remote machine to specified local folder (/bin default)");
            Add("scan", "", "Scans for links on the connected machine and adds them to the Map");
            Add("rm", "[filename (or use * for all files in folder)]", "Deletes specified file(s)");
            Add("ps", "", "Lists currently running processes and their PIDs");
            Add("kill", "[PID]", "Kills Process number [PID]");
            Add("ls", "", "Lists all files in current directory");
            Add("cd", "[foldername]", "Moves current working directory to the specified folder");
            Add("mv", "[FILE] [DESTINATION]", "Moves or renames [FILE] to [DESTINATION]\n(i.e: mv hi.txt ../bin/hi.txt)");
            Add("connect", "[ip]", "Connect to an External Computer");
            Add("probe", "", "Scans the connected machine for\nactive ports and security level");
            Add("exe", "", "Lists all available executables in the local /bin/ folder (Includes hidden and embedded executables)");
            Add("disconnect", "", "Terminate the current open connection. ALT: \"dc\"");
            Add("cat", "[filename]", "Displays contents of file");
            Add("openCDTray", "", "Opens the connected Computer's CD Tray");
            Add("closeCDTray", "", "Closes the connected Computer's CD Tray");
            Add("reboot", "[OPTIONAL: -i]", "Reboots the connected computer. The -i flag reboots instantly");
            Add("replace", "[filename] \"target\" \"replacement\"", "Replaces the target text in the file with the replacement");
            Add("analyze", "", "Performs an analysis pass on the firewall of the target machine");
            Add("solve", "[FIREWALL SOLUTION]", "Attempts to solve the firewall of target machine to allow UDP Traffic");
            Add("login", "", "Requests a username and password to log in to the connected system");
            Add("upload", "[LOCAL FILE PATH]", "Uploads the indicated file on your local machine to the current connected directory");
            Add("clear", "", "Clears the terminal");
            Add("addNote", "[NOTE]", "Add Note");
            Add("append", "[FILENAME] [DATA]", "Appends a line containing [DATA] to [FILENAME]");
            Add("shell", "", "Opens a remote access shell on target machine with Proxy overload\n and IP trap capabilities");
        }

        public static void Add(string key, string usage, string description)
        {
            Usages.Add(key, usage);
            Descriptions.Add(key, description);
        }

        public static bool Prefix()
        {
            Helpfile.help = new List<string>();
            Helpfile.postfix = "help " + LocaleTerms.Loc("[PAGE NUMBER]") + "\n " + LocaleTerms.Loc("Displays the specified page of commands.") + "\n---------------------------------\n";
            string ln = "\n    ";
            foreach (string k in Usages.Keys)
            {
                if (ZeroDayConditions.disabledCommands.Contains(k)) continue;
                Helpfile.help.Add(k + " " + LocaleTerms.Loc(Usages[k]) + ln + LocaleTerms.Loc(Descriptions[k]).Replace("\n", ln));
            }
            Helpfile.help.Sort();
            Helpfile.LoadedLanguage = Settings.ActiveLocale;
            return false;
        }
    }
}
