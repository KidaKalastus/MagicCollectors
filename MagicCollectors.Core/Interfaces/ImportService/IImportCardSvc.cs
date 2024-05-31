using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.ImportServices
{
    public interface IImportCardSvc
    {
        /// <summary>
        /// Get all cards of a set from Scryfall
        /// </summary>
        /// <param name="set"></param>
        /// <param name="firstRun"></param>
        /// <returns></returns>
        Task<List<Card>> Get(Set set);
    }
}