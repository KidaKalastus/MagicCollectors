using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Signers;
using System.Diagnostics;

namespace MagicCollectors.Services
{
    public class CollectionSvc : ICollectionSvc
    {
        private readonly IRepositorySvc repo;

        public CollectionSvc(IRepositorySvc repo)
        {
            this.repo = repo;
        }

        public async Task<List<CollectionCard>> Update(ApplicationUser collector, List<CollectionCard> updatedCards)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var timer = Stopwatch.StartNew();

                var dbCollector = await ctx.Users
                    .Include(x => x.CollectionCards)
                    .FirstAsync(x => x.Id == collector.Id);

                var timer1 = $"It took {timer.Elapsed.TotalSeconds} ms to get collector";
                timer = Stopwatch.StartNew();

                foreach (var updatedCard in updatedCards)
                {
                    var dbCollectionCard = dbCollector.CollectionCards.FirstOrDefault(x => x.CardId == updatedCard.Card.Id);

                    if (dbCollectionCard != null)
                    {
                        dbCollectionCard.Load(updatedCard);
                    }
                    else
                    {
                        dbCollector.CollectionCards.Add(updatedCard);
                    }
                }

                var timer2 = $"It took {timer.Elapsed.TotalSeconds} ms to update cards";
                timer = Stopwatch.StartNew();

                await ctx.SaveChangesAsync();

                var timer3 = $"It took {timer.Elapsed.TotalSeconds} ms to save changes";
                timer = Stopwatch.StartNew();

                repo.Reset($"{CacheKeys.CollectionSets}_{collector.Id}");
                repo.Reset($"{CacheKeys.CollectionCards}_{collector.Id}");

                var timer4 = $"It took {timer.Elapsed.TotalSeconds} ms to clear cache";
                timer = Stopwatch.StartNew();

                await UpdateCollectionSets(collector);

                var timer5 = $"It took {timer.Elapsed.TotalSeconds} ms to update sets";
                timer = Stopwatch.StartNew();

                var collection = await GetCollectionCards(collector);

                var timer6 = $"It took {timer.Elapsed.TotalSeconds} ms to get collection cards";
                timer = Stopwatch.StartNew();

                return collection.Where(x => updatedCards.Select(y => y.Id).Contains(x.Id)).ToList();
            }
        }

        public async Task<List<CollectionCard>> GetCollectionCards(ApplicationUser collector)
        {
            if (collector == null)
            {
                return [];
            }

            return await repo.Get<CollectionCard>(collector);
        }

        public async Task<List<CollectionSet>> GetCollectionSets(ApplicationUser collector)
        {
            if (collector == null)
            {
                return [];
            }

            return await repo.Get<CollectionSet>(collector);
        }

        public async Task UpdateCollectionSets(ApplicationUser collector)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var dbCollector = await ctx.Users
                    .Include(x => x.CollectionSets)
                    .FirstAsync(x => x.Id == collector.Id);

                var collectionCards = await repo.Get<CollectionCard>(collector);
                var setIds = collectionCards.Select(x => x.Card.Set).Select(x => x.Id).Distinct();
                foreach (var setId in setIds)
                {
                    var setCards = collectionCards.Where(x => x.Card.Set.Id == setId);
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

                    var existingSet = dbCollector.CollectionSets.FirstOrDefault(x => x.SetId == setId);
                    if (existingSet == null)
                    {
                        newSet.Set = await ctx.Sets.FirstAsync(x => x.Id == newSet.Set.Id);
                        dbCollector.CollectionSets.Add(newSet);
                        await ctx.SaveChangesAsync();
                        continue;
                    }

                    if (existingSet.SetHasChanged(newSet))
                    {
                        existingSet.Load(newSet);

                        var dbSet = await ctx.CollectionSets.FirstAsync(x => x.Id == existingSet.Id);
                        dbSet.Load(existingSet);

                        await ctx.SaveChangesAsync();
                    }
                }
            }
        }
    }
}