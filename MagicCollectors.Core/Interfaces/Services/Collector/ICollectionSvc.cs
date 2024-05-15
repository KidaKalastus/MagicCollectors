using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.Services
{
    public interface ICollectionSvc
    {
        /// <summary>
        /// Update collection with information about provided cards
        /// </summary>
        /// <param name="collector"></param>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<CollectionCard>> Update(ApplicationUser collector, List<CollectionCard> updatedCards);

        Task<List<CollectionCard>> GetCollectionCards(ApplicationUser collector);

        Task<List<CollectionSet>> GetCollectionSets(ApplicationUser collector);
    }
}