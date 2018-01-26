using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface ICatalogOperations
    {
        /// <summary>
        /// Retrieve a sorted, json list of repositories available in the registry.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Catalog> GetCatalogAsync(CatalogParameters parameters, CancellationToken cancellationToken = new CancellationToken());
    }
}