using System;
using BepInEx;
using BepInEx.Hacknet;
using Pathfinder.Port;
using Pathfinder.Replacements;
using Pathfinder.Util.XML;
using Pathfinder.Executable;
using Pathfinder.Command;

using ZeroDayToolKit.Executibles;
using ZeroDayToolKit.Commands;
using ZeroDayToolKit.Conditions;
using ZeroDayToolKit.TraceV2;
using ZeroDayToolKit.Utils;
using BepInEx.Configuration;
using Pathfinder.Meta;
using ZeroDayToolKit.Options;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using ZeroDayToolKit.Patches;
using Hacknet.Localization;
using Hacknet;
using Hacknet.Extensions;
using System.Collections.Generic;
using Hacknet.PlatformAPI.Storage;

namespace ZeroDayToolKit
{
    [BepInPlugin(ModGUID, ModName, ModVer)]
    [Updater("https://api.github.com/repos/prodzpod/ZeroDayToolKit/releases", "ZeroDayToolKit.Release.zip", "BepInEx/plugins/ZeroDayToolKit.dll", false)]
    public class ZeroDayToolKit : HacknetPlugin
    {
        public const string ModGUID = "kr.o_r.prodzpod.zerodaytoolkit";
        public const string ModName = "ZeroDayToolKit";
        public const string ModVer = "0.2.3";
        public new static ConfigFile Config;
        public static ZeroDayToolKit Instance;
        static public Random rnd;

        public override bool Load()
        {
            Instance = this;
            rnd = new Random();
            Config = base.Config;
            ZeroDayToolKitOptions.Initialize();
            ExtensionInfoLoader.AddLanguage("dynamic");
            var i = 0;
            foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes) {
                i++;
                if (type.GetCustomAttribute(typeof(HarmonyPatch)) != null) {
                        Log.LogDebug("Patching " + type);
                        HarmonyInstance.PatchAll(type);
                }
            }

            Console.WriteLine("[ZeroDayToolKit] Registering Ports");
            PortManager.RegisterPort("backdoor", "Backdoor Connection", 0);
            PortManager.RegisterPort("mqtt", "MQTT Protocol", 1883); // IoT Devices
            PortManager.RegisterPort("ntp", "Network Time Protocol", 123); // Digital Clocks > Timezones > VPN
            PortManager.RegisterPort("docker", "Docker Plaintext API", 2375);
            PortManager.RegisterPort("telnet", "Telnet Protocol", 23); // obviously these ports aint done yet

            Console.WriteLine("[ZeroDayToolKit] Registering Executables");
            ExecutableManager.RegisterExecutable<SSHSwiftEXE>("#SSH_SWIFT#");
            ExecutableManager.RegisterExecutable<PacketHeaderInjectionEXE>("#PACKET_HEADER_INJECTION#");
            ExecutableManager.RegisterExecutable<SQLTXCrasherEXE>("#SQL_TX_CRASHER#");
            ExecutableManager.RegisterExecutable<PortBackdoorEXE>("#PORT_BACKDOOR#");
            ExecutableManager.RegisterExecutable<MQTTInterceptorEXE>("#MQTT_INTERCEPTOR#");
            ExecutableManager.RegisterExecutable<GitTunnelEXE>("#GIT_TUNNEL#");

            Console.WriteLine("[ZeroDayToolKit] Registering Commands");
            BetterHelp.AddVanilla();
            ZeroDayCommand.Add("mkdir", MakeDir.Trigger, "[foldername]", "Creates an empty folder");
            ZeroDayCommand.Add("touch", Touch.Trigger, "[filename] [OPTIONAL: content]", "Creates a new file");
            ZeroDayCommand.Add("/", SendIRC.Trigger, "[message]", "Sends a text message to the active IRC");
            ZeroDayCommand.Add("btoa", Encode.generate("Base64", MathUtils.encodeBase64), "[FILE]", "Encodes the file to Base64");
            ZeroDayCommand.Add("atob", Decode.generate("Base64", MathUtils.decodeBase64), "[FILE]", "Decodes the file from Base64");
            var binencode = Encode.generate("Binary", MathUtils.encodeBinary, ".bin");
            var bindecode = Decode.generate("Binary", MathUtils.decodeBinary, "[EXT]");
            ZeroDayCommand.Add("binary", (os, args) =>
            {
                if (args.Length <= 2 || (args[1] != "-d" && args[1] != "-e")) os.write("Usage: binary [-e/-d] [FILE]");
                else if (args[1] == "-e") binencode(os, args.Skip(1).ToArray());
                else bindecode(os, args.Skip(1).ToArray());
            }, "[-d/-e] [FILE]", "Encodes and decodes the file to Binary");
            ZeroDayCommand.Add("zip", ZipEncode.Trigger, "[FOLDER]", "Compresses the folder into a file");
            ZeroDayCommand.Add("unzip", ZipDecode.Trigger, "[FILE]", "Decompresses the zip file into a folder");
            ZeroDayCommand.Add("echo", Echo.Trigger, "[content]", "Prints the content to the console");
            ZeroDayCommand.Add("date", Date.Trigger, "", "Prints the current time");
            ZeroDayCommand.Add("expr", Expr.Trigger, "[expression]", "Evaluates the given expression");
            ZeroDayCommand.Add("more", (os, args) => Programs.cat(args, os), "[filename]", "Displays contents of file");
            ZeroDayCommand.Add("head", Catlike.generate(false), "[filename] [OPTIONAL: number of lines]", "Displays the first part of file");
            ZeroDayCommand.Add("tail", Catlike.generate(true), "[filename] [OPTIONAL: number of lines]", "Displays the last part of file");
            ZeroDayCommand.Add("history", History.Trigger, "", "Prints the history of commands executed");
            ZeroDayCommand.Add("man", Man.Trigger, "[command]", "Prints the usage of that command");
            ZeroDayCommand.Add("hostname", Hostname.Trigger, "[OPTIONAL: -i]", "Prints the name or ip of the connected device");
            ZeroDayCommand.Add("pwd", Pwd.Trigger, "", "Prints the current working directory");

            Console.WriteLine("[ZeroDayToolKit] Registering Actions");
            Pathfinder.Action.ActionManager.RegisterAction<SAResetIRCDelay>("ResetIRCDelay");
            Pathfinder.Action.ActionManager.RegisterAction<SASetNumberOfChoices>("SetNumberOfChoices");
            Pathfinder.Action.ActionManager.RegisterAction<SARunCommand>("RunCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SASetRAM>("SetRAM");
            Pathfinder.Action.ActionManager.RegisterAction<SADisableCommand>("DisableCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SAEnableCommand>("EnableCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SAEnableStrictLog>("EnableStrictLog");
            Pathfinder.Action.ActionManager.RegisterAction<SADisableStrictLog>("DisableStrictLog");

            Console.WriteLine("[ZeroDayToolKit] Registering Conditions");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnFileCreation>("OnFileCreation");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnFileDeletion>("OnFileDeletion");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageAny>("OnIRCMessageAny");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessage>("OnIRCMessage");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageTone>("OnIRCMessageTone");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnHostileActionTaken>("OnHostileActionTaken");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnReboot>("OnReboot");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnCrash>("OnCrash");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnRebootCompleted>("OnRebootCompleted");

            Console.WriteLine("[ZeroDayToolKit] Register Complete");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("//////////////////////////////////// PRODZPOD");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("/////////////////////////////////// ZERODAY");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("////////////////////////////////// TOOLKIT");
            Console.ResetColor();
            return true;
        }
    }

    // triggers upon quitGame; resets things to default
    [HarmonyPatch(typeof(SaveFileManager), nameof(SaveFileManager.Init))] 
    public class ResetUponStart
    {
        public static void Prefix()
        {
            ZeroDayConditions.times = new Dictionary<string, TimeSpan>();
            ZeroDayConditions.choice = 3;
            ZeroDayConditions.disabledCommands = new List<string>
            {
                "/",
                "date",
                "source",
                "grep",
                "host",
                "updatedb",
                "find",
                "locate",
                "alias",
                "unalias",
                "diff"
            }; // initial disable list: a sort of power tool 
            Network.networks = new Dictionary<string, Network>();
            Network.postLoadComputerCache = new Dictionary<string, List<string>>();
            Network.afterCompleteTriggers = new List<AfterCompleteTrigger>();
            Network.tracker = new TraceV2Tracker();
            Network.recentHostileActionTaken = null;
            Network.recentReboot = null;
            Network.recentCrash = null;
            Network.recentRebootCompleted = null;
            Network.connections = 0;
            TrackerCheckLogs.stricts = new List<Computer>();
            ExtensionSequencerExeInstantActivate.queue = new List<ExtensionSequencerExe>();
            SequencerExeInstantActivate.queue = new List<SequencerExe>();
        }
    }
}