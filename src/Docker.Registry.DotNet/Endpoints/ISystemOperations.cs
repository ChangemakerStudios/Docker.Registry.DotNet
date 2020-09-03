using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface ISystemOperations
    {
        [PublicAPI]
        Task PingAsync(CancellationToken cancellationToken = default);
    }
}