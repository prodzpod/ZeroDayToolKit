using Pathfinder.Event.Saving;
using System;
using System.Linq;
using System.Xml.Linq;

namespace ZeroDayToolKit.Utils
{
    public static class SaveUtils
    {
        public static XElement Path(this SaveEvent e, string _path, Func<XElement, bool> fn = null)
        {
            if (_path.StartsWith("HacknetSave.")) _path = _path.Substring("HacknetSave.".Length);
            if (fn == null) fn = x => true;
            string[] path = _path.Split('.');
            return loop(e.Save, path, fn);
        }

        public static XElement loop(XElement e, string[] path, Func<XElement, bool> fn)
        {
            if (path.Length == 0)
            {
                if (fn(e)) return e;
                else return null;
            }
            else foreach (var child in e.Elements())
            {
                if (child.Name != path.First()) continue;
                var ret = loop(child, path.Skip(1).ToArray(), fn);
                if (ret != null) return ret;
            }
            return null;
        }
    }
}
