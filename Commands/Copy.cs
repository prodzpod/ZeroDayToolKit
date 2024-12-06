using System.Collections.Generic;
using System.Data;
using System.Linq;
using Hacknet;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Copy : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 3) { os.validCommand = false; os.write("Usage: cp [FILE] [FOLDER]"); return; }
            Computer computer = ComUtils.getComputer(os);
            Folder currentFolder = Programs.getCurrentFolder(os);
            var src = ComUtils.GetPath(os, args[1], os.navigationPath);
            var dest = ComUtils.GetPath(os, args[2], os.navigationPath);
            if (string.IsNullOrWhiteSpace(src.Name)) { os.validCommand = false; os.write("File is empty"); return; }
            if (string.IsNullOrWhiteSpace(dest.Name)) dest.Name = src.Name;
            if (src.Folder == dest.Folder && src.Name == dest.Name) { os.validCommand = false; os.write("Source and destination is identical"); return; }
            if (!CopyFile(computer, os.thisComputer.ip, src.FolderNavigation, src.Name, dest.FolderNavigation, dest.Name)) os.writeSingle(LocaleTerms.Loc("Error - Insufficient Privileges"));
            else os.writeSingle(LocaleTerms.Loc("Done"));
        }

        public static bool CopyFile(Computer computer, string ipFrom, List<int> srcPath, string srcName, List<int> destPath, string destName)
        {
            if (computer.currentUser.type != 0 && !computer.silent && !ipFrom.Equals(computer.adminIP) && !ipFrom.Equals(computer.ip)) return false;
            Folder srcFolder = Programs.getFolderFromNavigationPath(srcPath, computer.files.root, computer.os);
            Folder destFolder = Programs.getFolderFromNavigationPath(destPath, computer.files.root, computer.os);
            var src = srcFolder.searchForFile(srcName);
            if (src == null) return false;
            var dest = new FileEntry(src.data, destName);
            destFolder.files.Add(dest);
            computer.log("FileCopied: by " + ipFrom + " - file:" + srcName + " To: " + destName);
            return true;
        }
    }
}
