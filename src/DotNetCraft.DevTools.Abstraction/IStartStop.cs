using System.Threading;
using System.Threading.Tasks;

namespace DotNetCraft.DevTools.Abstraction
{
    public interface IStartStop
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
