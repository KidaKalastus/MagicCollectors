using MagicCollectors.Core.Data;
using MagicCollectors.Core.Interfaces.Services;
using MagicCollectors.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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
                var dbCollector = await ctx.Users
                    .Include(x => x.CollectionCards)
                    .FirstAsync(x => x.Id == collector.Id);

                foreach (var updatedCard in updatedCards)
                {
                    var dbCollectionCard = dbCollector.CollectionCards.FirstOrDefault(x => x.CardId == updatedCard.Card.Id);

                    if (dbCollectionCard != null && updatedCard.IsRelevant)
                    {
                        // The cards is already in collection and relevant
                        // It is updated
                        dbCollectionCard.Load(updatedCard);
                    }
                    else if (dbCollectionCard != null && !updatedCard.IsRelevant)
                    {
                        // The card is already in the collection but no longer relevant
                        // It is removed
                        ctx.CollectionCards.Remove(dbCollectionCard);
                    }
                    else if (updatedCard.IsRelevant)
                    {
                        // The card is not in the collection but is relevant
                        // It is added
                        dbCollector.CollectionCards.Add(updatedCard);
                    }
                }

                await ctx.SaveChangesAsync();

                repo.Reset($"{CacheKeys.CollectionSets}_{collector.Id}");
                repo.Reset($"{CacheKeys.CollectionCards}_{collector.Id}");

                await UpdateCollectionSets(collector);
                var collection = await GetCollectionCards(collector);
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

                // Delete collection sets no longer used
                foreach (var set in dbCollector.CollectionSets.Where(x => !setIds.Contains(x.SetId)))
                {
                    ctx.CollectionSets.Remove(set);
                }
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<CollectionCard>> Import(ApplicationUser collector, List<CollectionCard> cardsToAdd, bool deleteCollection)
        {
            var sets = await repo.Get<Set>();
            var cards = await repo.Get<Card>();

            var rejectedCollectionCards = new List<CollectionCard>();

            // The entire collection must be deleted first
            using (var ctx = new MagicCollectorsDbContext())
            {
                try
                {
                    if (deleteCollection)
                    {
                        var query = $"DELETE FROM CollectionCards WHERE ApplicationUserId = '{collector.Id}'";
                        await ctx.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));

                        query = $"DELETE FROM CollectionCards WHERE ApplicationUserId = '{collector.Id}'";
                        await ctx.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
                    }

                    repo.Reset($"{CacheKeys.CollectionSets}_{collector.Id}");
                    repo.Reset($"{CacheKeys.CollectionCards}_{collector.Id}");

                    collector = await ctx.Users
                        .Include(x => x.CollectionCards)
                        .Include(x => x.CollectionSets)
                        .FirstAsync(x => x.Id == collector.Id);

                    var collectionCards = collector.CollectionCards.ToDictionary(x => x.CardId);

                    foreach (var cardToAdd in cardsToAdd)
                    {
                        // Finding the set
                        var set = sets.FirstOrDefault(x => string.Compare(x.Code, cardToAdd.Card.Set.Code, true) == 0);
                        if (set == null)
                        {
                            set = sets.FirstOrDefault(x => string.Compare(x.Name, cardToAdd.Card.Set.Name, true) == 0);
                        }

                        if (set == null)
                        {
                            // If no set was found card must be rejected
                            rejectedCollectionCards.Add(cardToAdd);
                            continue;
                        }

                        // Loading cards of the set if not loaded
                        if (!set.Cards.Any())
                        {
                            set.Cards = cards.Where(x => x.SetId == set.Id).ToList();
                        }

                        // Finding the card
                        var card = set.Cards.FirstOrDefault(x => string.Compare(x.CollectorNumber, cardToAdd.Card.CollectorNumber, true) == 0);
                        if (string.IsNullOrEmpty(cardToAdd.Card.CollectorNumber) || card == null)
                        {
                            card = set.Cards.FirstOrDefault(x => string.Compare(x.Name, cardToAdd.Card.Name, true) == 0);
                        }

                        if (card == null)
                        {
                            // If no card was found card must be rejected
                            rejectedCollectionCards.Add(cardToAdd);
                            continue;
                        }

                        // Find information in database and add or insert
                        if (collectionCards.ContainsKey(card.Id))
                        {
                            var collectionCard = collectionCards[card.Id];
                            collectionCard.Count += cardToAdd.Count;
                            collectionCard.FoilCount += cardToAdd.FoilCount;
                            collectionCard.EtchedCount += cardToAdd.EtchedCount;
                        }
                        else
                        {
                            cardToAdd.CardId = card.Id;
                            cardToAdd.Card = null;
                            collector.CollectionCards.Add(cardToAdd);
                            collectionCards.Add(card.Id, cardToAdd);
                        }
                    }

                    await ctx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var something = ex.Message;
                }
            }

            repo.Reset($"{CacheKeys.CollectionSets}_{collector.Id}");
            repo.Reset($"{CacheKeys.CollectionCards}_{collector.Id}");

            await UpdateCollectionSets(collector);

            return rejectedCollectionCards;
        }

        public async Task<List<CollectionCard>> SearchForTrades(ApplicationUser collector, string emailOfOtherUser)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                // Get cards in collection to be searched
                var collectionCards = (await repo.Get<CollectionCard>(collector)).ToDictionary(x => x.CardId);

                // Get cards in collection needing cards
                var otherCollector = await ctx.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == emailOfOtherUser.ToLower());
                if (otherCollector == null)
                {
                    throw new Exception("No user with the provided email could be found");
                }
                var otherCollectionsCards = await repo.Get<CollectionCard>(otherCollector);

                var cardsWanted = new List<CollectionCard>();

                foreach (var card in otherCollectionsCards)
                {
                    if (card.Need <= 0)
                    {
                        // The card is not needed
                        continue;
                    }

                    if (!collectionCards.ContainsKey(card.CardId))
                    {
                        // The card is not in searched collection
                        continue;
                    }

                    var searchedCard = collectionCards[card.CardId];
                    if (searchedCard.Need >= 0)
                    {
                        // The card is wanted or only fulfilled in searched collection
                        continue;
                    }

                    var cardToAdd = new CollectionCard()
                    {
                        Card = card.Card,
                        CardId = card.CardId,
                    };

                    if (searchedCard.Trades <= card.Need)
                    {
                        // There are not as many as needed so we tell how many are available
                        cardToAdd.Count = searchedCard.Trades;
                    }
                    else
                    {
                        // There are more cards than needed so only provide how many are needed
                        cardToAdd.Count = card.Need;
                    }

                    cardsWanted.Add(cardToAdd);
                }

                return cardsWanted;
            }
        }

        public async Task<List<CollectionCard>> SearchForWantedCards(ApplicationUser collector, string emailOfOtherUser)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var otherCollector = await ctx.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == emailOfOtherUser.ToLower());
                if (otherCollector == null)
                {
                    throw new Exception("No user with the provided email could be found");
                }

                return await SearchForTrades(otherCollector, collector.Email);
            }
        }

        public async Task UpdateWants(ApplicationUser collector, int wantsCount, long setTypeId, bool includeVariants)
        {
            using (var ctx = new MagicCollectorsDbContext())
            {
                var updateQuery = @$"update CollectionCards Set want = {wantsCount}
                                        where ApplicationUserId = '{collector.Id}'";

                var insertQuery = @$"insert into CollectionCards
                                        (Want, Count, FoilCount, WantFoil, EtchedCount, WantEtched, CardId, ApplicationUserId)
                                        (select {wantsCount}, 0, 0, 0, 0, 0, Id, '{collector.Id}' From Cards
                                            where Id not in (select CardId from CollectionCards Where ApplicationUserId = '{collector.Id}')";

                if (setTypeId != 0)
                {
                    updateQuery = $"{updateQuery} and CardId in (select Id from Cards where SetId in (select Id from Sets where Type = {setTypeId}))";
                    insertQuery = $"{insertQuery} and SetId in (select Id from Sets where Type = {setTypeId})";
                }

                if (!includeVariants)
                {
                    updateQuery = $"{updateQuery} and CardId in (select Id from Cards where Extra = 0)";
                    insertQuery = $"{insertQuery} and Extra = 0";
                }

                insertQuery = $"{insertQuery})";

                await ctx.Database.ExecuteSqlRawAsync(updateQuery);
                await ctx.Database.ExecuteSqlRawAsync(insertQuery);

                repo.Reset($"{CacheKeys.CollectionSets}_{collector.Id}");
                repo.Reset($"{CacheKeys.CollectionCards}_{collector.Id}");

                await UpdateCollectionSets(collector);
            }
        }
    }
}