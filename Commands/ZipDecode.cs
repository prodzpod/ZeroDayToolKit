using System.Linq;
using System.Threading;
using Hacknet;

using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class ZipDecode : ZeroDayCommand
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
                os.write("You must input the file name");
                os.validCommand = false;
                return;
            }
            Folder folder = Programs.getCurrentFolder(os);
            FileEntry file = folder.searchForFile(args[1]) ?? folder.searchForFile(args[1] + ".zip");
            if (file == null)
            {
                os.write("Target file does not exist");
                os.validCommand = false;
                return;
            }
            unzipFolder(os, c, folder, file.data.Split('\n'));
        }

        public static void unzipFolder(OS os, Computer c, Folder source, string[] zip)
        {
            Folder folder = new Folder(ComUtils.getNoDupeFileName(zip[0], os));
            string idLog = "@" + (int)OS.currentElapsedTime;
            c.files.root.searchForFolder("log").files.Add(new FileEntry(idLog + " FolderCreated: by " + os.thisComputer.ip + " - folder:" + folder.name, idLog + "_FolderCreated:_by_" + os.thisComputer.ip + "_-_folder:" + folder.name));
            os.write("Unzipping " + folder.name);
            for (int i = 1; i < zip.Length; i++)
            {
                if (zip[i] == DELIM) break;
                if (zip[i].Contains(DELIM)) 
                {
                    string[] q = zip[i].Split(new[] { DELIM }, System.StringSplitOptions.None);
                    FileEntry file = new FileEntry();
                    file.name = ComUtils.getNoDupeFileName(q[0], os);
                    file.data = MathUtils.decodeZip(q[1]);
                    idLog = "@" + (int)OS.currentElapsedTime;
                    c.files.root.searchForFolder("log").files.Add(new FileEntry(idLog + " FileCreated: by " + os.thisComputer.ip + " - file:" + file.name, idLog + "_FileCreated:_by_" + os.thisComputer.ip + "_-_file:" + file.name));
                    os.write("Unzipping " + file.name);
                    folder.files.Add(file);
                    Thread.Sleep(200);
                }
                else
                {
                    int nest = 1;
                    int temp = i;
                    while (nest > 0)
                    {
                        i++;
                        if (zip[i] == DELIM) nest--;
                        else if (zip[i].Contains(DELIM)) nest++;
                    }
                    i++;
                    unzipFolder(os, c, folder, zip.Skip(temp).Take(i - temp).ToArray());
                }
            }
            source.folders.Add(folder);
        }
    }
}
