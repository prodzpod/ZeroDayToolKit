using System.Linq;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Touch : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Computer c = ComUtils.getComputer(os);
            if (!c.PlayerHasAdminPermissions()) os.write("Insufficient Permissions");
            else
            {
                if (args.Length < 1) os.write("You must input the file name");
                else
                {
                    {
                        string final_filename = FileFilter(args[1], os);
                        string textFile;
                        if (args.Length < 2) textFile = "";
                        else textFile = string.Join(" ", args.Skip(1));
                        string idLog = "@" + (int)OS.currentElapsedTime;
                        Programs.getCurrentFolder(os).files.Add(new FileEntry(textFile, final_filename));
                        // @[time]_FileCreated:_by_[ip]_-_file:[filename]
                        string logFilename = idLog + "_FileCreated:_by_" + os.thisComputer.ip + "_-_file:" + final_filename;
                        // @[time] FileCreated: by [ip] - file:[filename]
                        string logContent = idLog + " FileCreated: by " + os.thisComputer.ip + " - file:" + final_filename;
                        c.getFolderFromPath("/log").files.Add(new FileEntry(logContent, logFilename));
                    }
                }
            }
        }
        // stolen from xmod, thanks xmod
        public static string FileFilter(string filename, OS os)
        {
            int i_file;
            string filename_t = null;
            Folder actualFolder = Programs.getCurrentFolder(os);

            if (actualFolder.containsFile(filename))
            {
                if (!actualFolder.containsFile(filename + "(1)"))
                {
                    return filename + "(1)";
                }
                else
                {
                    i_file = 1;
                    while (actualFolder.containsFile(filename + "(" + i_file + ")"))
                    {
                        if (!actualFolder.containsFile(filename + "(" + (i_file + 1) + ")"))
                        {
                            i_file++;
                            filename_t = filename + "(" + i_file + ")";
                        }
                        else
                        {
                            i_file++;
                        }

                    }
                    return filename_t;
                }
            }
            else
            {
                return filename;
            }
        }
    }
}
