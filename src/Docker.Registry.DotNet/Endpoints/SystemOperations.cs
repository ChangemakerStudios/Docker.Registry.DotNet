using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Endpoints
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SystemOperations : ISystemOperations
    {
        private readonly NetworkClient _client;

        public SystemOperations(NetworkClient client)
        {
            _client = client;
        }

        public Task PingAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(cancellationToken, HttpMethod.Get, "v2/");
        }
    }
}