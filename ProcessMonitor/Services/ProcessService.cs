using System;
using System.Linq;
using System.Diagnostics;

namespace ProcessMonitor
{
    class ProcessService : IProcessService
    {
        private Process _process;
        private string _processName;
        public string ProcessName
        {
            get => _processName;
            set {
                if (_processName != value)
                {
                    _processName = value;
                    _process = Process.GetProcessesByName(_processName).FirstOrDefault();
                }
            }
        }

        public bool IsProcessRunning {
            get
            {
                _process?.Refresh();
                return _process != null && !_process.HasExited;
            }
        }
        public DateTime? ProcessStartTime => _process?.StartTime;

        public double ProcessLifetime => (DateTime.Now - _process.StartTime).TotalMinutes;

        public void ForceStopProcess() => _process?.Kill();

        public void WaitClosingProcess(int wait) 
        {
            if (_process.CloseMainWindow())
            {
                if(_process.WaitForExit(wait))
                {
                    return;
                }
            }
            _process.Kill();
        }
    }
}
