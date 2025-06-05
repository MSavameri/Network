using Microsoft.EntityFrameworkCore;
using NetworkInfrastructure.Web.Data.Context;
using NetworkInfrastructure.Web.Data.Entities;

namespace NetworkInfrastructure.Web.Data.Services
{
    public class NetworkService : INetworkService
    {
        private readonly NetworkContext _context;

        public NetworkService(NetworkContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NetworkAsset entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.NetworkAssets.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await GetAsync(id);
            
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            _context.Remove(result);

            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(NetworkAsset entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.NetworkAssets.Update(entity);

            await _context.SaveChangesAsync();  
        }

        public async Task<List<NetworkAsset>> GetAllAsync()
        {
            return await _context.NetworkAssets.AsNoTracking().ToListAsync();
        }

        public async Task<NetworkAsset?> GetAsync(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _context.NetworkAssets.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
