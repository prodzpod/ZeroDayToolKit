using Hacknet;
using Pathfinder.Command;
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
            BetterHelp.Add(name, usage, description);
            CommandManager.RegisterCommand(name, handler);
        }
    }
}
