using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    class SleepService : ISleepService
    {
        public Task Sleep(int interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }
    }
}
