using Hacknet;

namespace ZeroDayToolKit.TraceV2
{
    public class AfterCompleteTrigger : NetworkTrigger
    {
        public int every = 1;
        public int offAfter = -1;
        public int _offAfter;

        public override void Start(OS os, Network network, Computer source)
        {
            base.Start(os, network, source);
            _offAfter = offAfter;
        }

        public bool TryTrigger()
        {
            if (Network.connections % every != 0) return true;
            if (requireLogs && !Network.doesNetworkHaveLogsLeft(os, network)) return false;
            if (sourceIntact && !Network.doesNetworkHaveSourceIntact(os, network)) return false;
            if (_offAfter == 0) return false;
            Trigger();
            _offAfter--;
            return true;
        }
    }
}
