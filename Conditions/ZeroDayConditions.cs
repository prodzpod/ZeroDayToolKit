using System;
using System.Collections.Generic;

namespace ZeroDayToolKit.Conditions
{
    public abstract class ZeroDayConditions
    {
        public static Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>();
        public static int choice = 3;
        public static List<string> disabledCommands = new List<string> {
            "/",
            "date",
            "source",
            "grep",
            "host",
            "updatedb",
            "find",
            "locate",
            "alias",
            "unalias",
            "diff"
        };
    }
}
