using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.ImportServices
{
    public interface IImportSetSvc
    {
        /// <summary>
        /// Import all sets from Scryfall
        /// </summary>
        /// <returns></returns>
        Task<List<Set>> Get();
    }
}