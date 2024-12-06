using Hacknet;
using System.Threading;
using ZeroDayToolKit.Utils;

namespace ZeroDayToolKit.Commands
{
    public class Shutdown : ZeroDayCommand
    {
        public static new void Trigger(OS os, string[] args)
        {
            if (args.Length < 2 || !float.TryParse(args[1], out float s)) { os.validCommand = false; os.write("Usage: shutdown [seconds]"); return; }
            if (!os.hasConnectionPermission(admin: true)) { os.validCommand = false; os.write("Rebooting requires Admin access"); return; }
            os.write($"Shutdown Queued in {s} seconds.");
            Computer computer = ComUtils.getComputer(os);
            Thread.Sleep((int)(s * 1000));
            if (!os.HasExitedAndEnded)
            {
                if (computer == null || computer == os.thisComputer) os.rebootThisComputer();
                else computer.reboot(os.thisComputer.ip);
            }
        }
    }
}
