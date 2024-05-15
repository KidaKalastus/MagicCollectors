using MagicCollectors.Core.Interfaces.ImportServices;
using MagicCollectors.Core.Model;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MagicCollectors.ImportServices
{
    public class ImportSetSvc : IImportSetSvc
    {
        public async Task<List<Set>> Get()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.scryfall.com/sets")
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var path = "sets";

            var response = await client.GetAsync(path);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("An error occurred while getting sets from Scryfall");
            }

            var setCollection = JsonSerializer.Deserialize<ScryfallSetCollection>(responseString);

            return setCollection.data.Select(x => x.MapToCore()).ToList();
        }
    }
}