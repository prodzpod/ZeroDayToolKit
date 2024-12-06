using Hacknet;
using Hacknet.Daemons.Helpers;
using Pathfinder.Executable;
using Pathfinder.Port;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ZeroDayToolKit.Compat.Stuxnet;
using ZeroDayToolKit.Patches;

namespace ZeroDayToolKit.Utils
{
    public class ComUtils
    {
        public static Computer getComputer(OS os)
        {
            return os.connectedComp ?? os.thisComputer;
        }

        public static IRCSystem getIRC(Computer c)
        {
            IRCSystem irc = null;
            foreach (Daemon daemon in c.daemons)
            {
                if (daemon is DLCHubServer) irc = ((DLCHubServer)daemon).IRCSystem;
                else if (daemon is IRCDaemon) irc = ((IRCDaemon)daemon).System;
            }
            return irc;
        }

        public static bool isPortOpen(Computer c, int id)
        {
            return c.GetPortState(PortManager.GetPortRecordFromNumber(id).Protocol) != null;
        }
        public static bool hasLogOnSource(OS os, Computer c)
        {
            if (c == null) return false;
            if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(os, c, os.thisComputer.ip)) return true;
            return false;
        }
        public static bool isSourceIntact(OS os, Computer c)
        {
            if (c == null) return false;
            Folder sys = c.files.root.searchForFolder("sys");
            foreach (FileEntry file in sys.files) if (file.name == "netcfgx.dll" && file.data.Contains("0") && file.data.Contains("1")) return true;
            return false;
        }

        public static string getNoDupeFileName(string name, OS os) => getNoDupeFileName(name, Programs.getCurrentFolder(os));
        public static string getNoDupeFileName(string name, Folder folder)
        {
            if (!folder.containsFile(name) && folder.searchForFolder(name) == null) return name;
            for (int i = 1;; i++)
            {
                string newName = name + "(" + i + ")";
                if (!folder.containsFile(newName) && folder.searchForFolder(newName) == null) return newName;
            }
        }

        public static string getExtension(string data)
        {
            if (PortExploits.crackExeData.ContainsValue(data)) return ".exe";
            if (PortExploits.crackExeDataLocalRNG.ContainsValue(data)) return ".exe";
            if (ExecutableManager.AllCustomExes.Any(x => data == x.ExeData)) return ".exe";
            var image = ImageFile.Binaries.Keys.FirstOrDefault(x => ImageFile.Binaries[x] == data);
            if (image != null)
            {
                image = image.Trim().ToLower();
                if (image.EndsWith(".png")) return ".png";
                if (image.EndsWith(".jpg") || image.EndsWith(".jpeg")) return ".jpg";
                if (image.EndsWith(".gif")) return ".gif";
            }
            if (StuxnetCompat.Enabled)
            {
                var radio = StuxnetCompat.RadioBinaries.Keys.FirstOrDefault(x => StuxnetCompat.RadioBinaries[x] == data);
                if (radio != null) return ".ogg";
            }
            if (ThemeManager.fileData.ContainsValue(data)) return ".sys";
            if (data.Contains("<GitCommitEntry>")) return ".rec";
            if (data.Contains("##DHS_CONFIG")) return ".sys";
            if (data.Contains("RequireAuth:")) return ".cfg";
            if (data.EndsWith("--------------------")) return ".tm";
            if (data.Contains("<html>")) return ".html";
            if (data.StartsWith("@")) return "";
            if (data.Contains("MEMORY_DUMP : FORMAT")) return ".md";
            if (data.Contains("Archived Via : http://Bash.org")) return "";
            if (data.Contains("#DEC_ENC")) return ".dec";
            if (data.EndsWith("\n//")) return ".zip";
            if (data.StartsWith("#!/bin/sh") || data.StartsWith("#!/bin/bash") || data.StartsWith("#!/usr/bin/env bash")) return ".sh";
            if (MathUtils.decodeBinary(data) != null) return ".bin";
            return ".txt";
        }

        public struct PathEntry
        {
            public List<int> FolderNavigation;
            public Folder Folder;
            public string Name;
            public FileEntry File;
        }
        public static PathEntry GetPath(OS os, string path) => GetPath(os, path, []);
        public static PathEntry GetPath(OS os, Computer c, string path) => GetPath(os, c, path, []);
        public static PathEntry GetPath(OS os, string path, List<int> parent) => GetPath(os, getComputer(os), path, parent);
        public static PathEntry GetPath(OS os, Computer c, string path, List<int> parent)
        {
            List<int> folder;
            int num = path.LastIndexOf('/');
            if (num > 0) folder = Programs.getNavigationPathAtPath(path.Substring(0, num + 1), os, Programs.getFolderFromNavigationPath(parent, c.files.root, os));
            else folder = parent;
            string name = path.Substring(num + 1).Trim();
            PathEntry ret = new()
            {
                FolderNavigation = folder,
                Folder = Programs.getFolderFromNavigationPath(folder, c.files.root, os),
                Name = name
            };
            if (ret.Folder != null) ret.File = ret.Folder.searchForFile(ret.Name);
            return ret;
        }
    }
}
