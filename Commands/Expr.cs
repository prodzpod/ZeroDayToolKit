using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Hacknet;

namespace ZeroDayToolKit.Commands
{
    public class Expr : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {   
            if (args.Length < 2) { os.validCommand = false; os.write("Usage: expr [expression]"); }
            else try { os.write(new DataTable().Compute(string.Join(" ", args.Skip(1)), "").ToString()); }
            catch { os.write("Invalid Expression"); os.validCommand = false; }
        }
    }
}
