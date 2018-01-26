using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface ITagOperations
    {
        Task<ListImageTagsResponse> ListImageTagsAsync(string name, ListImageTagsParameters parameters, CancellationToken cancellationToken = new CancellationToken());
    }
}