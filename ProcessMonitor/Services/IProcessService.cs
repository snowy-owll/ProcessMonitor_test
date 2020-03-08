using System;

namespace ProcessMonitor
{
    public interface IProcessService
    {
        string ProcessName { get; set; }
        bool IsProcessRunning { get; }
        DateTime? ProcessStartTime { get; }
        double ProcessLifetime { get; }

        void ForceStopProcess();
        void WaitClosingProcess(int wait);
    }
}