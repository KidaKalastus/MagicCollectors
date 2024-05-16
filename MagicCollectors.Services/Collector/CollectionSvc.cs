using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.Caching;

namespace MagicCollectors.Services
{
    public class CollectionSvc : ICollectionSvc
    {
        private static MemoryCache cache;

        public CollectionSvc()
        {
            cache = MemoryCache.Default;
        }

        public async Task<List<CollectionCard>> Update(ApplicationUser collector, List<CollectionCard> updatedCards)
        {
            var collection = await GetCollectionCards(collector);
            foreach (var card in updatedCards)
            {
                // Check on the ID of the collection card
                if (collection.Any(x => x.Id == card.Id))
                {
                    using (var ctx = new MagicCollectorsDbContext())
                    {
                        var dbCard = await ctx.CollectionCards.FirstAsync(x => x.Id == card.Id);

                        dbCard.Load(card);

                        await ctx.SaveChangesAsync();
                    }
                    continue;
                }

                // Check on the ID of the actual card
                var collectionCard = collection.FirstOrDefault(x => x.Card.Id == card.Card.Id);
                if (collectionCard != null)
                {
                    collectionCard.Load(card);
                    using (var ctx = new MagicCollectorsDbContext())
                    {
                        var dbCard = await ctx.CollectionCards.FirstAsync(x => x.Id == collectionCard.Id);

                        dbCard.Load(card);

                        await ctx.SaveChangesAsync();
                    }
                    continue;
                }

                using (var ctx = new MagicCollectorsDbContext())
                {
                    var dbCollector = await ctx.Users
                        .Include(x => x.CollectionCards)
                        .FirstAsync(x => x.Id == collector.Id);

                    card.Card = await ctx.Cards.FirstAsync(x => x.Id == card.Card.Id);

                    dbCollector.CollectionCards.Add(card);

                    await ctx.SaveChangesAsync();
                }
            }

            cache.Remove($"{CacheKeys.CollectionCards}_{collector.Id}");

            collection = await GetCollectionCards(collector);

            DeleteCollectionSetsFromCache(collector);

            return collection.Where(x => updatedCards.Select(y => y.Id).Contains(x.Id)).ToList();
        }

        public async Task<List<CollectionCard>> GetCollectionCards(ApplicationUser collector)
        {
            if (collector == null)
            {
                return new List<CollectionCard>();
            }

            var cacheKey = $"{CacheKeys.CollectionCards}_{collector.Id}";

            if (cache.Contains(cacheKey))
            {
                return cache[cacheKey] as List<CollectionCard>;
            }

            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbCollector = await ctx.Users
                    .Include(x => x.CollectionCards)
                    .FirstOrDefaultAsync(x => x.Id == collector.Id);

                var collection = dbCollector.CollectionCards;

                foreach (var card in collection)
                {
                    if (card.Card != null && card.Card.Set != null)
                    {
                        card.Card.Set.Cards = null;
                    }
                }

                cache.Add(cacheKey, collection, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });

                return collection;
            }
        }

        private void DeleteCollectionSetsFromCache(ApplicationUser collector)
        {
            var cacheKey = $"{CacheKeys.CollectionSets}_{collector.Id}";
            cache.Remove(cacheKey);
        }

        public async Task<List<CollectionSet>> GetCollectionSets(ApplicationUser collector)
        {
            if (collector == null)
            {
                return new List<CollectionSet>();
            }

            var cacheKey = $"{CacheKeys.CollectionSets}_{collector.Id}";

            if (cache.Contains(cacheKey))
            {
                return cache[cacheKey] as List<CollectionSet>;
            }

            var collection = await UpdateCollectionSets(collector);

            cache.Add(cacheKey, collection, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });

            foreach (var set in collection)
            {
                if (set.Set != null)
                {
                    set.Set.Cards = null;
                }
            }

            return collection;
        }

        public async Task<List<CollectionSet>> UpdateCollectionSets(ApplicationUser collector)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbCollector = await ctx.Users
                    .Include(x => x.CollectionSets)
                    .FirstOrDefaultAsync(x => x.Id == collector.Id);

                var sets = dbCollector.CollectionSets;
                var cards = await GetCollectionCards(collector);

                var updatesExists = false;

                var allSets = cards.Select(x => x.Card.Set);

                foreach (var setId in allSets.Select(x => x.Id).Distinct())
                {
                    var setCards = cards.Where(x => x.Card.Set.Id == setId);
                    var newSet = new CollectionSet()
                    {
                        Set = new Set() { Id = setId }
                    };

                    foreach (var card in setCards)
                    {
                        newSet.Count += card.Count + card.FoilCount;
                        newSet.Want += card.Want + card.WantFoil;

                        if (card.Want > card.Count)
                        {
                            newSet.Missing += card.Want - card.Count;
                            newSet.CostOfMissingCards += (card.Want - card.Count) * card.Card.PriceUsd;
                        }

                        if (card.WantFoil > card.FoilCount)
                        {
                            newSet.Missing += card.WantFoil - card.FoilCount;
                            newSet.CostOfMissingCards += (card.WantFoil - card.FoilCount) * card.Card.PriceUsdFoil;
                        }

                        if (card.WantEtched > card.EtchedCount)
                        {
                            newSet.Missing += card.WantEtched - card.EtchedCount;
                            newSet.CostOfMissingCards += (card.WantEtched - card.EtchedCount) * card.Card.PriceUsdEtched;
                        }

                        newSet.ValueOfOwnedCards += (card.Count * card.Card.PriceUsd) + (card.FoilCount * card.Card.PriceUsdFoil) + (card.EtchedCount * card.Card.PriceUsdEtched);
                    }

                    var existingSet = sets.FirstOrDefault(x => x.Set.Id == setId);
                    if (existingSet == null)
                    {
                        newSet.Set = await ctx.Sets.FirstAsync(x => x.Id == newSet.Set.Id);
                        dbCollector.CollectionSets.Add(newSet);
                        await ctx.SaveChangesAsync();

                        updatesExists = true;
                        continue;
                    }

                    if (existingSet.SetHasChanged(newSet))
                    {
                        existingSet.Load(newSet);

                        var dbSet = await ctx.CollectionSets.FirstAsync(x => x.Id == existingSet.Id);
                        dbSet.Load(existingSet);

                        await ctx.SaveChangesAsync();
                        updatesExists = true;
                    }
                }

                if (updatesExists)
                {
                    dbCollector = await ctx.Users
                        .Include(x => x.CollectionSets.Select(x => x.Set))
                        .FirstOrDefaultAsync(x => x.Id == collector.Id);

                    return dbCollector.CollectionSets;
                }

                return sets;
            }
        }
    }
}