using MagicCollectors.Core.Model;
using System.Globalization;

namespace MagicCollectors.ImportServices
{
    public class ScryfallCard
    {
        public Guid id { get; set; }
        public Guid? oracle_id { get; set; }
        public long? tcgplayer_id { get; set; }
        public long? cardmarket_id { get; set; }
        public string name { get; set; }
        public DateTime released_at { get; set; }
        public string mana_cost { get; set; }
        public decimal? cmc { get; set; }
        public string line_type { get; set; }
        public string oracle_text { get; set; }
        public string[] finishes { get; set; }
        public string[] frame_effects { get; set; }
        public string border_color { get; set; }
        public bool foil { get; set; }
        public bool nonfoil { get; set; }
        public bool oversized { get; set; }
        public bool promo { get; set; }
        public string[] promo_types { get; set; }
        public string collector_number { get; set; }
        public string rarity { get; set; }
        public string flavor_text { get; set; }
        public Prices prices { get; set; }
        public ImageUrls image_uris { get; set; }

        public List<Faces> card_faces { get; set; }

        public Card MapToCore(bool extra = false)
        {
            Enum.TryParse(rarity, out Rarity cardRarity);

            var card = new Card()
            {
                Id = id,
                OracleId = oracle_id,
                TcgPlayerId = tcgplayer_id,
                CardMarketId = cardmarket_id,
                Name = name,
                ReleaseDate = released_at,
                ManaCost = mana_cost ?? string.Empty,
                ConvertedManaCost = cmc ?? 0.0m,
                SpellType = line_type ?? string.Empty,
                OracleText = oracle_text ?? string.Empty,
                Oversized = oversized,
                Promo = promo,
                CollectorNumber = collector_number,
                Rarity = cardRarity,
                FlavorText = flavor_text,
                Extra = extra,
                Foil = foil,
                NonFoil = nonfoil,
                BorderColor = border_color,
            };

            if (prices != null)
            {
                card.PriceUsd = Convert.ToDecimal(prices.usd, new CultureInfo("en-US"));
                card.PriceUsdFoil = Convert.ToDecimal(prices.usd_foil, new CultureInfo("en-US"));
                card.PriceUsdEtched = Convert.ToDecimal(prices.usd_etched, new CultureInfo("en-US"));

                card.PriceEuro = Convert.ToDecimal(prices.eur, new CultureInfo("en-US"));
                card.PriceEuroFoil = Convert.ToDecimal(prices.eur_foil, new CultureInfo("en-US"));

                card.PriceTix = Convert.ToDecimal(prices.tix, new CultureInfo("en-US"));
            }

            if (image_uris != null)
            {
                card.ImageDetails = image_uris.normal;
                //card.ImageDetails = card.ImageDetails.Replace("https://c1.scryfall.com/file/scryfall-cards/normal/front", "");
                card.ImageDetails = card.ImageDetails.Replace("https://cards.scryfall.io/normal/front", "");
            }
            else if (card_faces != null && card_faces.Any())
            {
                card.HasTwoFaces = true;
                card.ManaCost = string.Join("\\", card_faces.Select(x => x.mana_cost));
                foreach (var face in card_faces)
                {
                    if (face.image_uris == null)
                    {
                        continue;
                    }

                    if (face.image_uris.normal == null)
                    {
                        continue;
                    }

                    if (!face.image_uris.normal.Contains("/front/", StringComparison.CurrentCulture))
                    {
                        continue;
                    }

                    card.ImageDetails = face.image_uris.normal;
                    card.ImageDetails = card.ImageDetails.Replace("https://cards.scryfall.io/normal/front", "");
                }
            }

            if (promo_types != null)
            {
                foreach (var promo_type in promo_types)
                {
                    card.PromoTypes.Add(new PromoType() { Name = promo_type });
                }
            }

            if (finishes != null)
            {
                card.EtchedFoil = finishes.Any(x => x == "etched");
            }

            if (frame_effects != null)
            {
                foreach (var effect in frame_effects)
                {
                    card.FrameEffects.Add(new FrameEffect() { Name = effect });
                }
            }

            return card;
        }
    }

    public class ImageUrls
    {
        public string normal { get; set; }
    }

    public class Faces
    {
        public ImageUrls image_uris { get; set; }
        public string mana_cost { get; set; }
        public string name { get; set; }
        public string oracle_text { get; set; }
    }

    public class Prices
    {
        public string usd { get; set; }
        public string usd_foil { get; set; }
        public string usd_etched { get; set; }
        public string eur { get; set; }
        public string eur_foil { get; set; }
        public string tix { get; set; }
    }

    public class ScryfallCardCollection
    {
        public bool has_more { get; set; }
        public string next_page { get; set; }
        public List<ScryfallCard> data { get; set; }
    }
}