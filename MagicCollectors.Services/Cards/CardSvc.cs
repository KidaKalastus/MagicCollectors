using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace MagicCollectors.Services
{
    public class CardSvc : ICardSvc
    {
        private readonly IRepositorySvc repo;
        private readonly ICollectionSvc collectionSvc;

        public CardSvc(IRepositorySvc repo, ICollectionSvc collectionSvc)
        {
            this.repo = repo;
            this.collectionSvc = collectionSvc;
        }

        public Task<List<Card>> Get()
        {
            return repo.Get<Card>();
        }

        public async Task<List<CollectionCard>> Search(string query, ApplicationUser collector = null)
        {
            var cards = await repo.Get<Card>();

            var pattern = query.Replace(" ", ".*");
            cards = cards.Where(x => Regex.IsMatch(x.Name, pattern, RegexOptions.IgnoreCase)).ToList();

            return await LoadCardsWithCollectionInfo(cards, collector);
        }

        public async Task<List<CollectionCard>> Get(Set set, ApplicationUser collector = null)
        {
            var cards = await repo.Get<Card>();
            cards = cards.Where(x => string.Compare(x.Set.Code, set.Code, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            return await LoadCardsWithCollectionInfo(cards, collector);
        }

        public async Task<Card> Create(Card card)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbSet = await ctx.Sets.FirstAsync(x => x.Id == card.Set.Id);
                card.Set = dbSet;

                var dbPromoTypes = new List<PromoType>();
                foreach (var promoType in card.PromoTypes)
                {
                    var cardDbPromoType = await ctx.PromoTypes.FirstOrDefaultAsync(x => x.Name == promoType.Name);
                    if (cardDbPromoType == null)
                    {
                        ctx.PromoTypes.Add(promoType);
                        ctx.SaveChanges();
                    }
                    dbPromoTypes.Add(cardDbPromoType);
                }
                card.PromoTypes = dbPromoTypes;

                var dbFrameEffects = new List<FrameEffect>();
                foreach (var effects in card.FrameEffects)
                {
                    var cardDbEffect = await ctx.FrameEffects.FirstOrDefaultAsync(x => x.Name == effects.Name);
                    if (cardDbEffect == null)
                    {
                        ctx.FrameEffects.Add(effects);
                        ctx.SaveChanges();
                    }
                    dbFrameEffects.Add(cardDbEffect);
                }
                card.FrameEffects = dbFrameEffects;

                ctx.Cards.Add(card);
                await ctx.SaveChangesAsync();

                return card;
            }
        }

        public async Task<Card> Update(Card card)
        {
            repo.Reset(CacheKeys.Cards);

            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbCard = await ctx.Cards.FindAsync(card.Id);
                if (dbCard == null)
                {
                    return null;
                }

                ctx.Entry(dbCard).CurrentValues.SetValues(card);

                await ctx.SaveChangesAsync();

                return dbCard;
            }
        }

        public async Task Delete(Card card)
        {
            repo.Reset(CacheKeys.Cards);

            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbCard = await ctx.Cards.FindAsync(card.Id);

                ctx.Cards.Remove(dbCard);

                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<CollectionCard>> LoadCardsWithCollectionInfo(List<Card> cards, ApplicationUser collector)
        {
            var result = new List<CollectionCard>();

            // Load cards with collection info
            // if a collector is logged in result will come from collector
            // otherwise the list will be empty
            var collectionCards = await collectionSvc.GetCollectionCards(collector);
            foreach (var card in cards)
            {
                var collectionCard = collectionCards.FirstOrDefault(x => x.Card.Id == card.Id);

                if (collectionCard != null)
                {
                    result.Add(collectionCard);
                    continue;
                }

                result.Add(new CollectionCard()
                {
                    Card = card
                });
            }

            return result;
        }
    }
}