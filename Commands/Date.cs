using System;
using Hacknet;

namespace ZeroDayToolKit.Commands
{
    public class Date : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (!os.hasConnectionPermission(true)) { os.write("Insufficient Privileges to Perform Operation"); os.validCommand = false; }
            else os.write(DateTime.Now.ToLongDateString());
        }
    }
}
