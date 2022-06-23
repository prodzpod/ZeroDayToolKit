using Pathfinder.Util;
using Hacknet;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnFileCreation: Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public string target;
        [XMLStorage]
        public string path = "";
        [XMLStorage]
        public string name = "";
        [XMLStorage]
        public string content = "";

        public override bool Check(object os_obj)
        {
            Computer c = ComputerLookup.FindById(target);
            if (path == "") return CheckWholePC(c.files.root);
            else return CheckFolder(c.getFolderFromPath(path));
        }

        private bool CheckWholePC(Folder folder)
        {
            if (CheckFolder(folder)) return true;
            foreach (Folder child in folder.folders) if (CheckWholePC(child)) return true;
            return false;
        }

        private bool CheckFolder(Folder folder)
        {
            if (folder.files.Count == 0) return false;
            if (name != "" && !folder.containsFile(name)) return false;
            if (content != "" && !folder.containsFileWithData(content)) return false;
            return true;
        }
    }
}
