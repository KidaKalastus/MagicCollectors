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

        Task<List<CollectionCard>> Import(ApplicationUser collector, List<CollectionCard> cards, bool deleteCollection);

        /// <summary>
        /// Find cards the given collector has available for trade that other collector wants
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="emailOfOtherUser"></param>
        /// <returns></returns>
        Task<List<CollectionCard>> SearchForTrades(ApplicationUser collector, string emailOfOtherUser);

        /// <summary>
        /// Find cards the given collector wants among cards the other collector has
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="emailOfOtherUser"></param>
        /// <returns></returns>
        Task<List<CollectionCard>> SearchForWantedCards(ApplicationUser collector, string emailOfOtherUser);

        Task UpdateWants(ApplicationUser collector, int wantsCount, long setTypeId, bool includeVariants, bool onlyVariants);
    }
}