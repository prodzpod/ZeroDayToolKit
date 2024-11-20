using Pathfinder.Util;
using Hacknet;

namespace ZeroDayToolKit.Conditions
{
    public class SCOnFileCreation: Pathfinder.Action.PathfinderCondition
    {
        [XMLStorage]
        public string target;
        [XMLStorage]
        public string Target;
        [XMLStorage]
        public string path;
        [XMLStorage]
        public string Path = "";
        [XMLStorage]
        public string name;
        [XMLStorage]
        public string Name = "";
        [XMLStorage]
        public string content;
        [XMLStorage]
        public string Content = "";

        public override bool Check(object os_obj)
        {
            Computer c = ComputerLookup.FindById(target ?? Target);
            if ((path ?? Path) == "") return CheckWholePC(c.files.root);
            else return CheckFolder(c.getFolderFromPath((path ?? Path)));
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
            if ((name ?? Name) != "" && !folder.containsFile((name ?? Name))) return false;
            if ((content ?? Content) != "" && !folder.containsFileWithData((content ?? Content))) return false;
            return true;
        }
    }
}
