using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class MakeDir : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            if (!c.PlayerHasAdminPermissions()) os.write("Insufficient Permissions");
            else
            {
                if (args.Length < 1) os.write("You must input a valid folder name");
                else
                {
                    if (Programs.getCurrentFolder(os).searchForFolder(args[1]) == null)
                    {
                        string idLog = "@" + (int)OS.currentElapsedTime;
                        // @[time]_FileCreated:_by_[ip]_-_file:[filename]
                        string logFilename = idLog + "_FolderCreated:_by_" + os.thisComputer.ip + "_-_folder:" + args[1];
                        // @[time] FileCreated: by [ip] - file:[filename]
                        string logContent = idLog + " FolderCreated: by " + os.thisComputer.ip + " - folder:" + args[1];
                        c.getFolderFromPath("/log").files.Add(new FileEntry(logContent, logFilename));
                        Programs.getCurrentFolder(os).folders.Add(new Folder(args[1]));
                    }
                    else os.write("Folder already exists!");
                }
            }
        }
    }
}
