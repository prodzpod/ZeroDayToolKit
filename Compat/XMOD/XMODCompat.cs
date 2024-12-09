using BepInEx.Hacknet;
using Hacknet;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDayToolKit.Commands;

namespace ZeroDayToolKit.Compat.XMOD
{
    public class XMODCompat: ModCompats
    {
        public const string ID = "tenesiss.XMOD";
        public override string _ID => ID;
        public override void Init()
        {
            ZeroDayToolKit.Instance.Log.LogInfo("XMOD Detected: Loading Feature Compat");
        }
    }
}
