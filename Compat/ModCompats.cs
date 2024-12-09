using BepInEx.Hacknet;
using System.Collections.Generic;
using System.Reflection;

namespace ZeroDayToolKit.Compat
{
    public abstract class ModCompats
    {
        public static List<ModCompats> Compats = [];
        public virtual string _ID => "";
        public bool _Enabled => HacknetChainloader.Instance.Plugins.ContainsKey(_ID);
        public bool _Applied = false;
        public ModCompats()
        {
            if (!_Enabled || _Applied) return;
            Compats.Add(this);
            Init();
            _Applied = true;
        }
        public abstract void Init();
        public static ModCompats Get(string type) => Compats.Find(x => x.GetType().Name.EndsWith(type));
        public static bool Enabled(string type) => Compats.Find(x => x.GetType().Name.EndsWith(type))._Enabled;
        public static bool Applied(string type) => Compats.Find(x => x.GetType().Name.EndsWith(type))._Applied;
        public static void _Init()
        {
            foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes) if (type.IsSubclassOf(typeof(ModCompats)))
            {
                var inst = (ModCompats)type.GetConstructor([]).Invoke(null);
                if (inst._Applied) Compats.Add(inst);
            }
        }
    }
}
