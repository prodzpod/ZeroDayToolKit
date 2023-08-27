using System.Threading;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class ZipEncode : ZeroDayCommand
    {
        public const string DELIM = "//";
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            if (!c.PlayerHasAdminPermissions())
            {
                os.write("Insufficient Permissions");
                os.validCommand = false;
                return;
            }
            if (args.Length < 2)
            {
                os.write("You must input the folder name");
                os.validCommand = false;
                return;
            }
            Folder source = Programs.getCurrentFolder(os);
            Folder target = source.searchForFolder(args[1]);
            if (target == null)
            {
                os.write("Target folder does not exist");
                os.validCommand = false;
                return;
            }
            c.makeFile(os.thisComputer.ip, ComUtils.getNoDupeFileName(args[1] + ".zip", os), zipFolder(target), os.navigationPath);
        }

        public static string zipFolder(Folder folder)
        {
            string ret = folder.name + "\n";
            foreach (Folder target in folder.folders) ret += zipFolder(target) + "\n";
            foreach (FileEntry file in folder.files) ret += zipFile(file) + "\n";
            return ret + DELIM;
        }

        public static string zipFile(FileEntry file)
        {
            Thread.Sleep(200);
            return file.name + DELIM + MathUtils.encodeZip(file.data);
        }
    }
}
