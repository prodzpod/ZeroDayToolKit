using System;
using System.Linq;
using Hacknet;

namespace ZeroDayToolKit.Commands
{
    public class Catlike : ZeroDayCommand
    {
        public bool isTail = false;
        public static Action<OS, string[]> generate(bool isTail)
        {
            Catlike ret = new()
            {
                isTail = isTail
            };
            return ret.Trigger;
        }
        public new void Trigger(OS os, string[] args)
        {
            if (os.hasConnectionPermission(admin: true))
            {
                os.displayCache = "";
                Folder currentFolder = Programs.getCurrentFolder(os);
                if (args.Length < 2)
                {
                    os.validCommand = false;
                    os.write("Usage: " + (isTail ? "tail" : "head") + " [filename] [OPTIONAL: lines]");
                }
                else
                {
                    for (int i = 0; i < currentFolder.files.Count; i++)
                    {
                        if (!currentFolder.files[i].name.Equals(args[1])) continue;
                        if (
                            (os.connectedComp != null)
                            ? os.connectedComp.canReadFile(os.thisComputer.ip, currentFolder.files[i], i)
                            : os.thisComputer.canReadFile(os.thisComputer.ip, currentFolder.files[i], i))
                        {
                            var nr = args.Length < 3 ? 10 : int.Parse(args[2]);
                            if (nr <= 0) nr = 1;
                            string[] lines = LocalizedFileLoader.SafeFilterString(currentFolder.files[i].data).Split('\n');
                            os.write(string.Join("\n", isTail ? lines.Skip(lines.Length - nr) : lines.Take(nr)));
                            os.display.LastDisplayedFileFolder = currentFolder;
                            os.display.LastDisplayedFileSourceIP = ((os.connectedComp != null) ? os.connectedComp.ip : os.thisComputer.ip);
                        }
                        return;
                    }
                    os.validCommand = false;
                    os.write("File Not Found\n");
                }
            }
            else
            {
                os.validCommand = false;
                os.write("Insufficient Privileges to Perform Operation");
            }
        }
    }
}
