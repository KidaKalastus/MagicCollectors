using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.Services
{
    public interface ISetSvc
    {
        /// <summary>
        /// Get all sets
        /// </summary>
        /// <returns></returns>
        Task<List<CollectionSet>> Get(bool sync, ApplicationUser collector = null);

        /// <summary>
        /// Get information about sets from Scryfall and add or update sets in database
        /// </summary>
        /// <returns></returns>
        Task Sync(bool sync = false);
    }
}