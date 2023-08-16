using NetworkInfrastructure.Web.Data.Entities;

namespace NetworkInfrastructure.Web.Data.Services
{
    public interface INetworkService
    {
        Task<List<NetworkAsset>> GetAllAsync();

        Task<NetworkAsset> GetAsync(Guid id);

        Task AddAsync(NetworkAsset entity);

        Task DeleteAsync(Guid id);

        Task EditAsync(NetworkAsset entity);
    }
}
