using System.Threading;
using System.Threading.Tasks;
using Catalyst.Core.Lib.DAO.Ledger;

namespace Catalyst.Core.Modules.Sync.Interface
{
    public interface IDeltaHeightWatcher
    {
        DeltaIndexDao LatestDeltaHash { get; }
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task WaitForDeltaHeightAsync(int currentDeltaIndex);
    }
}
