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
        private readonly ICollectionSvc collectionSvc;
        private static MemoryCache cache;

        public CardSvc(ICollectionSvc collectionSvc)
        {
            this.collectionSvc = collectionSvc;
            cache = MemoryCache.Default;
        }

        public async Task<List<CollectionCard>> Search(string query, ApplicationUser collector = null)
        {
            var cards = await GetFromCache();

            var pattern = query.Replace(" ", ".*");
            cards = cards.Where(x => Regex.IsMatch(x.Name, pattern, RegexOptions.IgnoreCase)).ToList();

            return await LoadCardsWithCollectionInfo(cards, collector);
        }

        public async Task<List<CollectionCard>> Get(Set set, ApplicationUser collector = null)
        {
            var cards = await GetFromCache();
            cards = cards.Where(x => string.Compare(x.Set.Code, set.Code, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            return await LoadCardsWithCollectionInfo(cards, collector);
        }

        public async Task<Card> Create(Card card)
        {
            cache.Remove(CacheKeys.Cards);

            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbSet = await ctx.Sets.FirstAsync(x => x.Id == card.Set.Id);
                card.Set = dbSet;

                var dbFinishes = new List<Finish>();
                foreach (var finish in card.Finishes)
                {
                    var cardDbFinish = await ctx.Finishes.FirstOrDefaultAsync(x => x.Name == finish.Name);
                    if (cardDbFinish == null)
                    {
                        ctx.Finishes.Add(finish);
                        ctx.SaveChanges();
                    }
                    dbFinishes.Add(cardDbFinish);
                }
                card.Finishes = dbFinishes;

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
            cache.Remove(CacheKeys.Cards);

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
            cache.Remove(CacheKeys.Cards);

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

        private async Task<List<Card>> GetFromCache()
        {
            if (cache.Contains(CacheKeys.Cards))
            {
                return cache[CacheKeys.Cards] as List<Card>;
            }

            using (var ctx = new MagicCollectorsDbContext())
            {
                var cards = await ctx.Cards
                    .Include(x => x.Set)
                    .Include(x => x.Finishes)
                    .Include(x => x.PromoTypes)
                    .Include(x => x.FrameEffects)
                    .ToListAsync();

                foreach (var card in cards)
                {
                    if (card.Set != null)
                    {
                        card.Set.Cards = null;
                    }
                }

                cache.Add(CacheKeys.Cards, cards, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
                return cards;
            }
        }

        public Task<List<Card>> Get()
        {
            return GetFromCache();
        }
    }
}