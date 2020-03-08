using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    [Command(Name = "processmonitor", Description = "The utility checks the lifetime of the process and ends it if it lives for a long time.")]
    [HelpOption("-?|-h|--help")]
    public class ProcessMonitorApp
    {
        private readonly IConsole _console;
        private readonly IProcessService _processService;
        private readonly ISleepService _sleepService;

        [Argument(0, Description = "Required. Process name.")]
        [Required]
        public string ProcessName { get; set; }

        [Argument(1, Description = "Required. Process lifetime (in minutes).")]
        [Required]
        [Integer]
        [Min(1, ErrorMessage = "Lifetime cannot be less than 1")]
        public string Lifetime { get; set; }

        [Argument(2, Description = "Required. Process lifetime check frequency (in minutes).")]
        [Required]
        [Integer]
        [Min(1, ErrorMessage = "Rate cannot be less than 1")]
        public string Rate { get; set; }

        [Option(Description = "Optional. Wait for normal closing process.")]
        public bool WaitClosing { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "Optional. Closing timeout (in seconds)")]
        [Integer]
        [Min(1, ErrorMessage = "Closing timeout cannot be less than 1")]
        public string ClosingTimeout { get; set; } = "10";

        public ProcessMonitorApp(IConsole console, IProcessService processService, ISleepService sleepService)
        {
            _console = console;
            _processService = processService;
            _sleepService = sleepService;
        }

        public async Task<int> OnExecute(CancellationToken cancellationToken)
        {
            _processService.ProcessName = ProcessName;
            try
            {
                if (!_processService.IsProcessRunning)
                {
                    _console.WriteLine($"The process '{ProcessName}' does not running");
                    return 1;
                }
                if (CheckProcessLifetime())
                    _console.WriteLine("Waiting for lifetime to exceed... To cancel press Ctrl+C.");
                else
                {
                    _console.WriteLine("The process lifetime has already been exceeded. The process is killed.");
                    return 0;
                }
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_processService.IsProcessRunning)
                    {
                        _console.WriteLine($"The process '{ProcessName}' was stopped while waiting.");
                        return 1;
                    }
                    if (!CheckProcessLifetime())
                    {
                        _console.WriteLine($"The process {ProcessName} was successfully stopped.");
                        return 0;
                    }
                    await _sleepService.Sleep(int.Parse(Rate) * 60 * 1000, cancellationToken);
                }
            }
            catch (Win32Exception)
            {
                _console.WriteLine($"Access denied to process '{ProcessName}'");
                return 1;
            }
            catch (Exception)
            {
                if(_processService.IsProcessRunning)
                    _console.WriteLine($"An error occurred while working with the process. Process '{ProcessName}' is running.");
                else
                    _console.WriteLine($"An error occurred while working with the process. Process '{ProcessName}' stopped.");
                return 1;
            }
            return 0;
        }

        private bool CheckProcessLifetime()
        {
            if (_processService.ProcessLifetime > int.Parse(Lifetime))
            {
                if (WaitClosing) _processService.WaitClosingProcess(int.Parse(ClosingTimeout) * 1000);
                else _processService.ForceStopProcess();
                return false;
            }
            return true;
        }
    }
}
