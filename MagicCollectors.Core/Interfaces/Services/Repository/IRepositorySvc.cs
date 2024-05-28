using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.Services
{
    public interface IRepositorySvc
    {
        Task<List<T>> Get<T>();

        Task<List<T>> Get<T>(ApplicationUser applicationUser);

        void Reset(string cacheKey);
    }
}