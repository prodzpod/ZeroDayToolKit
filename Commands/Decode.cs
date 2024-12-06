using System;
using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Decode : ZeroDayCommand
    {
        public string name;
        public string ext;
        public Func<string, string> func;
        public static Action<OS, string[]> generate(string name, Func<string, string> func, string ext = "_DECODED[EXT]")
        {
            Decode decode = new()
            {
                func = func,
                name = name,
                ext = ext
            };
            return decode.Trigger;
        }

        public new void Trigger(OS os, string[] args)
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
                os.write("You must input the file name");
                os.validCommand = false;
                return;
            }
            Folder folder = Programs.getCurrentFolder(os);
            string truncName = string.Join(".", args[1].Split('.').Reverse().Skip(1).Reverse());
            FileEntry file = folder.searchForFile(args[1]) ?? folder.searchForFile(truncName);
            if (file == null)
            {
                os.write("File does not exist");
                os.validCommand = false;
                return;
            }
            string decoded = func(file.data);
            if (decoded == null)
            {
                os.write("File is not a valid " + name + " format");
                os.validCommand = false;
                return;
            }
            c.makeFile(os.thisComputer.ip, ComUtils.getNoDupeFileName(truncName + ext.Replace("[EXT]", ComUtils.getExtension(decoded)), os), decoded, os.navigationPath);
        }
    }
}
