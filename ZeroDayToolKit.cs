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

namespace ZeroDayToolKit
{
    [BepInPlugin(ModGUID, ModName, ModVer)]
    public class ZeroDayToolKit : HacknetPlugin
    {
        public const string ModGUID = "kr.o_r.prodzpod.zerodaytoolkit";
        public const string ModName = "ZeroDayToolKit";
        public const string ModVer = "0.1.1";
        static public Random rnd;

        public override bool Load()
        {
            rnd = new Random();
            HarmonyInstance.PatchAll();

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
            CommandManager.RegisterCommand("mkdir", MakeDir.Trigger);
            CommandManager.RegisterCommand("touch", Touch.Trigger);
            CommandManager.RegisterCommand("/", SendIRC.Trigger);
            CommandManager.RegisterCommand("btoa", Encode.generate("Base64", MathUtils.encodeBase64));
            CommandManager.RegisterCommand("atob", Decode.generate("Base64", MathUtils.decodeBase64));
            CommandManager.RegisterCommand("binencode", Encode.generate("Binary", MathUtils.encodeBinary, ".bin"));
            CommandManager.RegisterCommand("bindecode", Decode.generate("Binary", MathUtils.decodeBinary, "[EXT]"));
            CommandManager.RegisterCommand("zip", ZipEncode.Trigger);
            CommandManager.RegisterCommand("unzip", ZipDecode.Trigger);

            Console.WriteLine("[ZeroDayToolKit] Registering Actions");
            Pathfinder.Action.ActionManager.RegisterAction<SAResetIRCDelay>("ResetIRCDelay");
            Pathfinder.Action.ActionManager.RegisterAction<SASetNumberOfChoices>("SetNumberOfChoices");
            Pathfinder.Action.ActionManager.RegisterAction<SARunCommand>("RunCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SASetRAM>("SetRAM");
            Pathfinder.Action.ActionManager.RegisterAction<SADisableCommand>("DisableCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SAEnableCommand>("EnableCommand");

            Console.WriteLine("[ZeroDayToolKit] Registering Conditions");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageAny>("OnIRCMessageAny");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessage>("OnIRCMessage");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageTone>("OnIRCMessageTone");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnHostileActionTaken>("OnHostileActionTaken");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnReboot>("OnReboot");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnCrash>("OnCrash");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnRebootCompleted>("OnRebootCompleted");

            Console.WriteLine("[ZeroDayToolKit] Registering Mechanisms");
            SaveLoader.RegisterExecutor<LoadTraceV2>("HacknetSave.TraceV2", ParseOption.ParseInterior);

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
}