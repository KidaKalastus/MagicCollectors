using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.ImportServices;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Caching;

namespace MagicCollectors.Services
{
    public class SetSvc : ISetSvc
    {
        private readonly IImportSetSvc importSetSvc;
        private readonly IImportCardSvc importCardSvc;
        private readonly ICardSvc cardSvc;
        private readonly ICollectionSvc collectionSvc;
        private static MemoryCache cache;

        public SetSvc(IImportSetSvc importSetSvc, IImportCardSvc importCardSvc, ICardSvc cardSvc, ICollectionSvc collectionSvc)
        {
            this.importSetSvc = importSetSvc;
            this.importCardSvc = importCardSvc;
            this.cardSvc = cardSvc;
            this.collectionSvc = collectionSvc;
            cache = MemoryCache.Default;
        }

        public async Task Sync()
        {
            try
            {
                var dbSets = new List<Set>();

                using (var ctx = new MagicCollectorsDbContext())
                {
                    dbSets = await ctx.Sets.ToListAsync();
                    var scryfallSets = await importSetSvc.Get();

                    foreach (var scryfallSet in scryfallSets)
                    {
                        var existingSet = dbSets.FirstOrDefault(x => x.Id == scryfallSet.Id);
                        if (existingSet != null)
                        {
                            ctx.Entry(existingSet).CurrentValues.SetValues(scryfallSet);
                        }
                        else
                        {
                            ctx.Sets.Add(scryfallSet);
                        }
                    }

                    foreach (var dbSet in dbSets)
                    {
                        var scryfallSet = scryfallSets.FirstOrDefault(x => x.Id == dbSet.Id);
                        if (scryfallSet == null)
                        {
                            ctx.Sets.Remove(dbSet);
                        }
                    }

                    await ctx.SaveChangesAsync();

                    dbSets = await ctx.Sets.ToListAsync();
                }

                var count = 0;
                var pageSize = 30;

                foreach (var set in dbSets)
                {
                    try
                    {
                        using (var ctx = new MagicCollectorsDbContext())
                        {
                            var currentSet = await ctx.Sets.FirstAsync(x => x.Id == set.Id);
                            var promoTypes = await ctx.PromoTypes.ToDictionaryAsync(x => x.Name, x => x);
                            var frameEffects = await ctx.FrameEffects.ToDictionaryAsync(x => x.Name, x => x);
                            var cards = await importCardSvc.Get(currentSet);
                            var setCards = await ctx.Cards.Where(x => x.Set.Id == currentSet.Id).ToListAsync();

                            foreach (var card in cards)
                            {
                                card.Set = null;
                                card.SetId = currentSet.Id;
                                var dbCard = setCards.FirstOrDefault(x => x.Id == card.Id);
                                if (dbCard != null)
                                {
                                    // Update card
                                    ctx.Entry(dbCard).CurrentValues.SetValues(card);
                                }
                                else
                                {
                                    var dbPromoTypes = new List<PromoType>();
                                    foreach (var promoType in card.PromoTypes)
                                    {
                                        if (!promoTypes.ContainsKey(promoType.Name))
                                        {
                                            ctx.PromoTypes.Add(promoType);
                                            await ctx.SaveChangesAsync();
                                            promoTypes = await ctx.PromoTypes.ToDictionaryAsync(x => x.Name, x => x);
                                        }
                                        dbPromoTypes.Add(promoTypes[promoType.Name]);
                                    }
                                    card.PromoTypes = dbPromoTypes;

                                    var dbFrameEffects = new List<FrameEffect>();
                                    foreach (var effect in card.FrameEffects)
                                    {
                                        if (!frameEffects.ContainsKey(effect.Name))
                                        {
                                            ctx.FrameEffects.Add(effect);
                                            await ctx.SaveChangesAsync();
                                            frameEffects = await ctx.FrameEffects.ToDictionaryAsync(x => x.Name, x => x);
                                        }
                                        dbFrameEffects.Add(frameEffects[effect.Name]);
                                    }
                                    card.FrameEffects = dbFrameEffects;

                                    ctx.Cards.Add(card);
                                }
                            }

                            foreach (var dbCard in setCards)
                            {
                                if (!cards.Any(x => x.Id == dbCard.Id))
                                {
                                    await cardSvc.Delete(dbCard);
                                }
                            }

                            await ctx.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        var something = ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                var something = ex.ToString();
            }
        }

        public async Task<List<CollectionSet>> Get(bool sync, ApplicationUser collector = null)
        {
            if (sync)
            {
                await Sync();
            }

            var sets = (await GetFromCache()).OrderBy(x => x.ReleaseDate).ToList();

            return await LoadSetsWithCollectionInfo(sets, collector);
        }

        public async Task<List<CollectionSet>> LoadSetsWithCollectionInfo(List<Set> sets, ApplicationUser collector)
        {
            var result = new List<CollectionSet>();

            // Load cards with collection info
            // if a collector is logged in the result will come from collector
            // otherwise the list is empty
            var collectionSets = await collectionSvc.GetCollectionSets(collector);
            foreach (var set in sets)
            {
                var collectionSet = collectionSets.FirstOrDefault(x => x.Set.Id == set.Id);

                if (collectionSet != null)
                {
                    result.Add(collectionSet);
                    continue;
                }

                result.Add(new CollectionSet()
                {
                    Set = set,
                });
            }

            return result;
        }

        private async Task<List<Set>> GetFromCache()
        {
            if (cache.Contains(CacheKeys.Sets))
            {
                return cache[CacheKeys.Sets] as List<Set>;
            }

            using (var ctx = new MagicCollectorsDbContext())
            {
                var sets = await ctx.Sets.ToListAsync();
                cache.Add(CacheKeys.Sets, sets, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
                return sets;
            }
        }
    }
}