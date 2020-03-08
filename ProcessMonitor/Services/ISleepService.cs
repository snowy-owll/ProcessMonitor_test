using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitor
{
    public interface ISleepService
    {
        Task Sleep(int interval, CancellationToken cancellationToken);
    }
}