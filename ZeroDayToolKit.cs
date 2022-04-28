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

namespace ZeroDayToolKit
{
    [BepInPlugin(ModGUID, ModName, ModVer)]
    public class ZeroDayToolKit : HacknetPlugin
    {
        public const string ModGUID = "kr.o_r.prodzpod.modname";
        public const string ModName = "ModName";
        public const string ModVer = "0.1.0";
        static public Random rnd;

        public override bool Load()
        {
            rnd = new Random();
            HarmonyInstance.PatchAll();

            PortManager.RegisterPort("backdoor", "Backdoor Connection", 0);
            PortManager.RegisterPort("mqtt", "MQTT Protocol", 1883); // IoT Devices
            PortManager.RegisterPort("ntp", "Network Time Protocol", 123); // Digital Clocks > Timezones > VPN
            PortManager.RegisterPort("docker", "Docker Plaintext API", 2375);
            PortManager.RegisterPort("telnet", "Telnet Protocol", 23); // obviously these ports aint done yet

            ExecutableManager.RegisterExecutable<SSHSwiftEXE>("#SSH_SWIFT#");
            ExecutableManager.RegisterExecutable<PacketHeaderInjectionEXE>("#PACKET_HEADER_INJECTION#");
            ExecutableManager.RegisterExecutable<SQLTXCrasherEXE>("#SQL_TX_CRASHER#");
            ExecutableManager.RegisterExecutable<PortBackdoorEXE>("#PORT_BACKDOOR#");

            CommandManager.RegisterCommand("mkdir", MakeDir.Trigger);
            CommandManager.RegisterCommand("touch", Touch.Trigger);
            CommandManager.RegisterCommand("/", SendIRC.Trigger);

            Pathfinder.Action.ActionManager.RegisterAction<SAResetIRCDelay>("ResetIRCDelay");
            Pathfinder.Action.ActionManager.RegisterAction<SASetNumberOfChoices>("SetNumberOfChoices");
            Pathfinder.Action.ActionManager.RegisterAction<SARunCommand>("RunCommand");
            Pathfinder.Action.ActionManager.RegisterAction<SASetRAM>("SetRAM");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageAny>("OnIRCMessageAny");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessage>("OnIRCMessage");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnIRCMessageTone>("OnIRCMessageTone");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnHostileActionTaken>("OnHostileActionTaken");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnReboot>("OnReboot");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnCrash>("OnCrash");
            Pathfinder.Action.ConditionManager.RegisterCondition<SCOnRebootCompleted>("OnRebootCompleted");

            SaveLoader.RegisterExecutor<LoadTraceV2>("HacknetSave.TraceV2", ParseOption.ParseInterior);

            Console.WriteLine("//////////////////////////////////// PRODZPOD");
            Console.WriteLine("/////////////////////////////////// ZERODAY");
            Console.WriteLine("////////////////////////////////// TOOLKIT");
            return true;
        }
    }
}