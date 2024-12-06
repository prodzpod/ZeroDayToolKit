using System;
using System.IO;
using System.Xml;
using Hacknet;
using Hacknet.Extensions;

namespace ZeroDayToolKit.TraceV2
{
    [HarmonyLib.HarmonyPatch(typeof(ExtensionLoader), nameof(ExtensionLoader.LoadNewExtensionSession))] // load from extension to os
    public class InitialLoadTraceV2
    {
        static void Prefix(ExtensionInfo info, object os_obj)
        {
            OS os = (OS)os_obj;
            if (Directory.Exists(info.FolderPath + "/Networks"))
            {
                Hacknet.Utils.ActOnAllFilesRevursivley(info.FolderPath + "/Networks", filename =>
                {
                    if (!filename.EndsWith(".xml")) return;
                    Console.WriteLine("Reading " + filename);
                    XmlReader rdr = XmlReader.Create(File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(filename)));
                    while (rdr.Name != "TraceV2")
                    {
                        rdr.Read();
                        if (rdr.EOF) return;
                    }
                    Network network = new();
                    string name;
                    if (rdr.MoveToAttribute("name"))
                    {
                        name = rdr.ReadContentAsString();
                        Network.networks[name] = network;
                        Network.postLoadComputerCache[name] = [];
                    }
                    else return;
                    if (rdr.MoveToAttribute("head")) Network.postLoadComputerCache[name].Add(rdr.ReadContentAsString());
                    else return;
                    rdr.Read();
                    while (rdr.Name != "TraceV2")
                    {
                        if (rdr.Name.ToLower().Equals("trace") && rdr.MoveToAttribute("time")) network.traceTime = rdr.ReadContentAsFloat();
                        if (rdr.Name.ToLower().Equals("reboot") && rdr.MoveToAttribute("time")) network.rebootTime = rdr.ReadContentAsFloat();
                        if (rdr.Name.ToLower().Equals("computer") && rdr.MoveToAttribute("name")) Network.postLoadComputerCache[name].Add(rdr.ReadContentAsString());
                        if (rdr.Name.ToLower().Equals("onstart") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onStart = new()
                            {
                                action = rdr.ReadContentAsString()
                            };
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onStart.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onStart.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onStart.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onStart.delay = rdr.ReadContentAsFloat();
                            network.onStart = onStart;
                        }
                        if (rdr.Name.ToLower().Equals("oncrash") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onCrash = new()
                            {
                                action = rdr.ReadContentAsString()
                            };
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onCrash.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onCrash.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onCrash.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onCrash.delay = rdr.ReadContentAsFloat();
                            network.onCrash = onCrash;
                        }
                        if (rdr.Name.ToLower().Equals("oncomplete") && rdr.MoveToAttribute("action"))
                        {
                            NetworkTrigger onComplete = new()
                            {
                                action = rdr.ReadContentAsString()
                            };
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) onComplete.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) onComplete.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) onComplete.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) onComplete.delay = rdr.ReadContentAsFloat();
                            network.onComplete = onComplete;
                        }
                        if (rdr.Name.ToLower().Equals("aftercomplete") && rdr.MoveToAttribute("action"))
                        {
                            AfterCompleteTrigger afterComplete = new()
                            {
                                action = rdr.ReadContentAsString()
                            };
                            if (rdr.MoveToAttribute("RequireLogsOnSource")) afterComplete.requireLogs = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("RequireSourceIntact")) afterComplete.sourceIntact = rdr.ReadContentAsBoolean();
                            if (rdr.MoveToAttribute("DelayHost")) afterComplete.delayHost = rdr.ReadContentAsString();
                            if (rdr.MoveToAttribute("Delay")) afterComplete.delay = rdr.ReadContentAsFloat();
                            if (rdr.MoveToAttribute("every")) afterComplete.every = rdr.ReadContentAsInt();
                            if (rdr.MoveToAttribute("offAfter")) afterComplete.offAfter = rdr.ReadContentAsInt();
                            network.afterComplete = afterComplete;
                        }
                        if (rdr.EOF) return;
                        rdr.Read();
                    }
                    rdr.Close();
                    ComputerLoader.postAllLoadedActions += () =>
                    {
                        if (Network.postLoadComputerCache.Count != 0) foreach (string key in Network.postLoadComputerCache.Keys)
                            {
                                var value = Network.postLoadComputerCache[key];
                                Network.networks[key].head = Programs.getComputer(ComputerLoader.os, value[0]);
                                foreach (string id in value) Network.networks[key].tail.Add(Programs.getComputer(ComputerLoader.os, id));
                            }
                    };
                });
            }
        }
    }
}
