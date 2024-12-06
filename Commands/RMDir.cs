using Hacknet;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using ZeroDayToolKit.Utils;
using System.Collections;

namespace ZeroDayToolKit.Commands
{
    public class RMDir : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length <= 1) { os.validCommand = false; os.write("Not Enough Arguments\n"); return; }
            Computer computer = os.connectedComp ?? os.thisComputer;
            Folder currentFolder = Programs.getCurrentFolder(os);
            Folder folder = Programs.getFolderAtPath(args[1], os, currentFolder, returnsNullOnNoFind: true);
            if (folder == null) { os.validCommand = false; os.write("Folder " + args[1] + " Not found!"); return; }
            var nav = Programs.getNavigationPathAtPath(args[1], os, currentFolder);
            Folder parent = Programs.getFolderFromNavigationPath(nav.Range(0, -1).ToList(), computer.files.root, os);
            os.write(LocaleTerms.Loc("Deleting") + " " + folder.name + ".");
            ZeroDayToolKit.Instance.Log.LogInfo(parent.name + " > " + folder.name);
            for (int j = 0; j < 15; j++)
            {
                Thread.Sleep(200);
                os.writeSingle(".");
            }
            if (!DeleteFolder(os, computer, os.thisComputer.ip, folder.name, parent))
            {
                os.validCommand = false; 
                os.writeSingle(LocaleTerms.Loc("Error - Insufficient Privileges"));
            }
            else os.writeSingle(LocaleTerms.Loc("Done"));
            os.write("");
        }

        public static bool DeleteFolder(OS os, Computer computer, string ipFrom, string name, Folder parent)
        {
            if (
                computer.currentUser.type != 0
                && !computer.silent 
                && !ipFrom.Equals(computer.adminIP) 
                && !ipFrom.Equals(computer.ip)) return false;
            if (parent == computer.files.root && new string[] { "home", "log", "bin", "sys" }.Contains(name)) return false;
            // DeleteFolderInternal(parent, Programs.getFolderAtPath(name, computer.os, parent));
            parent.folders.Remove(Programs.getFolderAtPath(name, os, parent));
            computer.log("FolderDeleted: by " + ipFrom + " - folder:" + name);
            return true;
        }

        /*
        public static void DeleteFolderInternal(Folder parent, Folder folder)
        {
            foreach (var child in folder.folders) DeleteFolderInternal(folder, child);
            foreach (var file in folder.files) 
        }
        */
    }
}
