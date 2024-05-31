using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace MagicCollectors.Services
{
    public class RepositorySvc : IRepositorySvc
    {
        private static MemoryCache cache;

        public RepositorySvc()
        {
            cache = MemoryCache.Default;
        }

        public async Task<List<T>> Get<T>()
        {
            var type = typeof(T);

            // Get cards
            if (type == typeof(Card))
            {
                if (cache.Contains(CacheKeys.Cards))
                {
                    return (List<T>)cache[CacheKeys.Cards];
                }

                using (var ctx = new MagicCollectorsDbContext())
                {
                    var sets = await ctx.Sets.ToDictionaryAsync(x => x.Id, x => x);
                    var cards = await ctx.Cards.Include(x => x.FrameEffects).ToListAsync();
                    foreach (var card in cards)
                    {
                        card.Set = sets[card.SetId];
                    }
                    cache.Add(CacheKeys.Cards, cards, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
                    return cards as List<T> ?? [];
                }
            }
            else if (type == typeof(Set))
            {
                if (cache.Contains(CacheKeys.Sets))
                {
                    return (List<T>)cache[CacheKeys.Sets];
                }

                using (var ctx = new MagicCollectorsDbContext())
                {
                    var sets = await ctx.Sets.ToListAsync();
                    cache.Add(CacheKeys.Sets, sets, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
                    return sets as List<T> ?? [];
                }
            }

            return new List<T>();
        }

        public async Task<List<T>> Get<T>(ApplicationUser collector)
        {
            var type = typeof(T);

            // Get Collection cards
            if (type == typeof(CollectionCard))
            {
                var cacheKey = $"{CacheKeys.CollectionCards}_{collector.Id}";

                if (cache.Contains(cacheKey))
                {
                    return (List<T>)cache[cacheKey];
                }

                var cards = (await Get<Card>()).ToDictionary(x => x.Id);
                using (var ctx = new MagicCollectorsDbContext())
                {
                    var dbCollector = await ctx.Users
                        .Include(x => x.CollectionCards)
                        .FirstAsync(x => x.Id == collector.Id);

                    var collection = dbCollector.CollectionCards;
                    foreach (var card in collection)
                    {
                        card.Card = cards[card.CardId];
                    }
                    cache.Add(cacheKey, collection, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });

                    return collection as List<T>;
                }
            }

            // Get Collection sets
            if (type == typeof(CollectionSet))
            {
                var cacheKey = $"{CacheKeys.CollectionSets}_{collector.Id}";

                if (cache.Contains(cacheKey))
                {
                    return (List<T>)cache[cacheKey];
                }

                using (var ctx = new MagicCollectorsDbContext())
                {
                    var dbCollector = await ctx.Users
                        .Include(x => x.CollectionSets)
                        .FirstAsync(x => x.Id == collector.Id);

                    var dbSets = await ctx.Sets.ToDictionaryAsync(x => x.Id);

                    foreach (var set in dbCollector.CollectionSets)
                    {
                        set.Set = dbSets[set.SetId];
                    }

                    cache.Add(cacheKey, dbCollector.CollectionSets, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });

                    return dbCollector.CollectionSets as List<T>;
                }
            }

            return [];
        }

        public void Reset(string cacheKey)
        {
            cache.Remove(cacheKey);
        }

        public void Reset()
        {
            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }
        }
    }
}