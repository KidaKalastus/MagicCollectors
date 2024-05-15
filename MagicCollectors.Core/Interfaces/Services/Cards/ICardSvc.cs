using MagicCollectors.Core.Model;

namespace MagicCollectors.Core.Interfaces.Services
{
    public interface ICardSvc
    {
        Task<List<CollectionCard>> Search(string query, ApplicationUser collector = null);

        Task<List<CollectionCard>> Get(Set set, ApplicationUser collector = null);

        Task<List<Card>> Get();

        Task<Card> Create(Card card);

        Task<Card> Update(Card card);

        Task Delete(Card card);
    }
}