using Hacknet;
using Pathfinder.Command;
using Pathfinder.Util;
using System;
using ZeroDayToolKit.Patches;

namespace ZeroDayToolKit.Commands
{
    public abstract class ZeroDayCommand
    {
        public static string usage => "";
        public static string description => "";
        public static void Trigger(OS os, string[] args) { }

        public static void Add(string name, Action<OS, string[]> handler, string usage, string description)
        {
            try { CommandManager.RegisterCommand(name, handler); }
            catch { return; }
            BetterHelp.Add(name, usage, description);
        }
    }
}
