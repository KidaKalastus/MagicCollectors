using MagicCollectors.Core.Interfaces.ImportServices;
using MagicCollectors.Core.Model;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MagicCollectors.ImportServices
{
    public class ImportCardSvc : IImportCardSvc
    {
        public async Task<List<Card>> Get(Set set, bool firstRun = true)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.scryfall.com/")
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var path = $"cards/search?q=set%3a{set.Code}";

            // All is done twice to get Extra-cards on the second run.
            // Otherwise these cannot be marked as they have no unique variables
            if (!firstRun)
            {
                path = $"{path}+unique%3Aprints";
            }

            var response = await client.GetAsync(path);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<Card>();
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("An error occurred while getting cards from Scryfall");
            }

            var setCollection = JsonSerializer.Deserialize<ScryfallCardCollection>(responseString);

            var result = setCollection.data.Select(x => x.MapToCore()).ToList();

            while (setCollection.has_more)
            {
                response = await client.GetAsync(setCollection.next_page);
                responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("An error occurred while getting cards from Scryfall");
                }
                setCollection = JsonSerializer.Deserialize<ScryfallCardCollection>(responseString);
                result.AddRange(setCollection.data.Select(x => x.MapToCore()).ToList());
            }

            if (firstRun)
            {
                var allCards = await Get(set, false);
                var extras = allCards.Where(x => !result.Select(y => y.Id).Contains(x.Id)).ToList();
                foreach (var extra in extras)
                {
                    extra.Extra = true;
                }
                result.AddRange(extras);
            }

            return result;
        }
    }
}