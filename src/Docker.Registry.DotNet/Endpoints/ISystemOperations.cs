using System.Threading;
using System.Threading.Tasks;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface ISystemOperations
    {
        Task PingAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}