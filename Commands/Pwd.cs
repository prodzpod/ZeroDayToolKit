using Hacknet;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Pwd : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            Folder f = ComUtils.getComputer(os).files.root;
            string ret = "";
            for (int i = 0; i < os.navigationPath.Count; i++)
            {
                f = f.folders[os.navigationPath[i]];
                ret += "/" + f.name;
            }
            if (ret == "") ret += "/";
            os.write(ret);
        }
    }
}
