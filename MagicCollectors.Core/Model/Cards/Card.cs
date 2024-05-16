using System.ComponentModel.DataAnnotations;

namespace MagicCollectors.Core.Model
{
    /// <summary>
    /// Card objects represent individual Magic: The Gathering cards that players could obtain and add to their collection (with a few minor exceptions).
    /// </summary>
    public class Card
    {
        #region Scryfall data

        /// <summary>
        /// A unique ID for this card in Scryfall’s database.
        /// Scryfall: id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        public Guid? OracleId { get; set; }
        public long? TcgPlayerId { get; set; }
        public long? CardMarketId { get; set; }

        /// <summary>
        /// The name of this particular related card.
        /// Scryfall: name
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The date this card was first released.
        /// Scryfall: released_at
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Some cards have two faces
        /// </summary>
        public bool HasTwoFaces { get; set; } = false;

        /// <summary>
        /// The mana cost for this card. This value will be any empty string "" if the cost is absent.
        /// Remember that per the game rules, a missing mana cost and a mana cost of {0} are different values.
        /// Multi-faced cards will report this value in card faces.
        /// Scryfall: mana_cost
        /// </summary>
        public string ManaCost { get; set; } = string.Empty;

        /// <summary>
        /// The card’s converted mana cost. Note that some funny cards have fractional mana costs.
        /// Scryfall: cmc
        /// </summary>
        public decimal ConvertedManaCost { get; set; } = decimal.Zero;

        /// <summary>
        /// The type line of this card.
        /// Scryfall: type_line
        /// </summary>
        public string SpellType { get; set; } = string.Empty;

        /// <summary>
        /// The Oracle text for this card, if any.
        /// Scryfall: oracle_text
        /// </summary>
        public string OracleText { get; set; } = string.Empty;

        /// <summary>
        /// The color and style of the border of the card
        /// Scryfall: border_color
        /// </summary>
        public string BorderColor { get; set; } = string.Empty;

        /// <summary>
        /// A list of effects used on the frame
        /// Scryfall: frame_effects
        /// </summary>
        public List<FrameEffect> FrameEffects { get; set; } = [];

        /// <summary>
        /// Wether or not the card exists in a foil version
        /// Scryfall: foil
        /// </summary>
        public bool Foil { get; set; } = false;

        /// <summary>
        /// Wether or not the card exists in a non-foil version
        /// Scryfall: nonfoil
        /// </summary>
        public bool NonFoil { get; set; } = false;

        /// <summary>
        /// Wether or not the card exists in a etched foil version
        /// </summary>
        public bool EtchedFoil { get; set; } = false;

        /// <summary>
        /// True if this card is a promotional print.
        /// Scryfall: promo
        /// </summary>
        public bool Promo { get; set; } = false;

        /// <summary>
        /// An array of strings describing what categories of promo cards this card falls into.
        /// Scryfall: promo_types
        /// </summary>
        public List<PromoType> PromoTypes { get; set; } = [];

        /// <summary>
        /// Info used to generate links for images
        /// An image uri is comprised of several parts
        /// 1: Scryfall address: https://c1.scryfall.com/file/scryfall-cards
        /// 2: Image type: "/art_crop/front", "/border_crop/front", "/large/front", "/normal/front", "/png/front" and "/small/front"
        /// 3: Image details: /2/4/ or similar.
        /// 4: Card ID: Must be lower-case
        /// 5: Extension: ".jpg" for all but "/png/front" which is ... ".png"
        /// The last three parts are contained in this attribute
        /// </summary>
        public string ImageDetails { get; set; } = string.Empty;

        public string ImageUrlSmall
        {
            //https://cards.scryfall.io/normal/front/7/b/7becaa04-f142-4163-9286-00018b95c4ca.jpg?1601138543
            get { return $"https://cards.scryfall.io/small/front{ImageDetails}"; }
        }

        public string ImageUrlNormal
        {
            get { return $"https://cards.scryfall.io/normal/front{ImageDetails}"; }
        }

        public string ImageUrlLarge
        {
            get { return $"https://cards.scryfall.io/large/front{ImageDetails}"; }
        }

        public string ImageUrlSmallBack
        {
            get { return $"https://cards.scryfall.io/small/back{ImageDetails}"; }
        }

        public string ImageUrlNormalBack
        {
            get { return $"https://cards.scryfall.io/normal/back{ImageDetails}"; }
        }

        public string ImageUrlLargeBack
        {
            get { return $"https://cards.scryfall.io/large/back{ImageDetails}"; }
        }

        /// <summary>
        /// True if this card is oversized.
        /// Scryfall: oversized
        /// </summary>
        public bool Oversized { get; set; } = false;

        /// <summary>
        /// True if the card is extra in the set and not a regular card
        /// </summary>
        public bool Extra { get; set; } = false;

        /// <summary>
        /// The set of the card
        /// </summary>
        public Set Set { get; set; }

        public Guid SetId { get; set; }

        /// <summary>
        /// This card’s collector number. Note that collector numbers can contain non-numeric characters, such as letters or ★.
        /// Scryfall: collector_number
        /// </summary>
        public string? CollectorNumber { get; set; }

        /// <summary>
        /// This card’s rarity. One of common, uncommon, rare, special, mythic, or bonus.
        /// Scryfall: rarity
        /// </summary>
        public Rarity Rarity { get; set; } = Rarity.Undefined;

        /// <summary>
        /// The flavor text, if any.
        /// Scryfall: flavor_text
        /// </summary>
        public string? FlavorText { get; set; }

        /// <summary>
        /// The price in USD
        /// </summary>
        public decimal PriceUsd { get; set; } = decimal.Zero;

        /// <summary>
        /// The price in USD for foil
        /// </summary>
        public decimal PriceUsdFoil { get; set; } = decimal.Zero;

        /// <summary>
        /// The price in USD for etched foil card
        /// </summary>
        public decimal PriceUsdEtched { get; set; } = decimal.Zero;

        /// <summary>
        /// Price in EURO
        /// </summary>
        public decimal PriceEuro { get; set; } = decimal.Zero;

        /// <summary>
        /// Price in EURO for foil
        /// </summary>
        public decimal PriceEuroFoil { get; set; } = decimal.Zero;

        /// <summary>
        /// Price on MTGO
        /// </summary>
        public decimal PriceTix { get; set; } = decimal.Zero;

        public List<CollectionCard> CollectionCards { get; set; } = new List<CollectionCard>();

        /*

        Used API:
        https://scryfall.com/docs/api
        https://scryfall.com/docs/api/cards

        Other APIs
        https://docs.tcgplayer.com/docs
        https://docs.magicthegathering.io/

        Raw data from Scryfall. -- in front of variable means it has been implemented.
        For full list see: https://scryfall.com/docs/api/cards

        -- "id": "504698a9-1512-4288-b5ef-392d41ebcd05",
        -- "oracle_id": "a8f53018-c496-41f3-bc3f-3c4703b9e4e1",
        "multiverse_ids": [ 489604 ],
        "arena_id": 72182,
        -- "tcgplayer_id": 216370,
        -- "cardmarket_id": 473774,
        -- "name": "Long Road Home",
        "lang": "en",
        -- "released_at": "2020-07-17",
        "uri": "https://api.scryfall.com/cards/504698a9-1512-4288-b5ef-392d41ebcd05",
        "scryfall_uri": "https://scryfall.com/card/jmp/120/long-road-home?utm_source=api",
        "layout": "normal",
        "highres_image": true,
        "image_status": "highres_scan",
        "image_uris": {
            "small": "https://c1.scryfall.com/file/scryfall-cards/small/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.jpg?1600696905",
            "normal": "https://c1.scryfall.com/file/scryfall-cards/normal/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.jpg?1600696905",
            "large": "https://c1.scryfall.com/file/scryfall-cards/large/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.jpg?1600696905",
            "png": "https://c1.scryfall.com/file/scryfall-cards/png/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.png?1600696905",
            "art_crop": "https://c1.scryfall.com/file/scryfall-cards/art_crop/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.jpg?1600696905",
            "border_crop": "https://c1.scryfall.com/file/scryfall-cards/border_crop/front/5/0/504698a9-1512-4288-b5ef-392d41ebcd05.jpg?1600696905"
        },
        -- "mana_cost": "{1}{W}",
        -- "cmc": 2,
        -- "type_line": "Instant",
        -- "oracle_text": "Exile target creature. At the beginning of the next end step, return that card to the battlefield under its owner's control with a +1/+1 counter on it.",
        "colors": [ "W" ],
        "color_identity": [ "W" ],
        "keywords": [],
        "legalities": {
            "standard": "not_legal",
            "future": "not_legal",
            "historic": "legal",
            "gladiator": "legal",
            "pioneer": "legal",
            "modern": "legal",
            "legacy": "legal",
            "pauper": "not_legal",
            "vintage": "legal",
            "penny": "legal",
            "commander": "legal",
            "brawl": "not_legal",
            "historicbrawl": "legal",
            "alchemy": "not_legal",
            "paupercommander": "not_legal",
            "duel": "legal",
            "oldschool": "not_legal",
            "premodern": "not_legal"
        },
        "games": [ "arena", "paper"],
        "reserved": false,
        "foil": false,
        "nonfoil": true,
        -- "finishes": [ "nonfoil" ],
        -- "oversized": false,
        -- "promo": false,
        "reprint": true,
        "variation": false,
        -- "set_id": "0f6ccf25-a627-4263-86df-5757137f1696",
        -- set": "jmp",
        -- "set_name": "Jumpstart",
        -- "set_type": "draft_innovation",
        -- "set_uri": "https://api.scryfall.com/sets/0f6ccf25-a627-4263-86df-5757137f1696",
        -- "set_search_uri": "https://api.scryfall.com/cards/search?order=set&q=e%3Ajmp&unique=prints",
        "scryfall_set_uri": "https://scryfall.com/sets/jmp?utm_source=api",
        "rulings_uri": "https://api.scryfall.com/cards/504698a9-1512-4288-b5ef-392d41ebcd05/rulings",
        "prints_search_uri": "https://api.scryfall.com/cards/search?order=released&q=oracleid%3Aa8f53018-c496-41f3-bc3f-3c4703b9e4e1&unique=prints",
        -- "collector_number": "120",
        "digital": false,
        -- "rarity": "uncommon",
        -- "flavor_text": "The person who finishes a journey is rarely the same as the one who starts it.",
        "card_back_id": "0aeebaf5-8c7d-4636-9e82-8c27447861f7",
        "artist": "Sidharth Chaturvedi",
        "artist_ids": [ "55e6d846-2f73-4fba-9b88-441686bb8dcb" ],
        "illustration_id": "5ca2e4d7-50a1-4580-9c1f-055b14c25c41",
        -- "border_color": "black",
        "frame": "2015",
        "full_art": false,
        "textless": false,
        "booster": true,
        "story_spotlight": false,
        "edhrec_rank": 5456,
        "preview": {
            "source": "Wizards of the Coast",
            "source_uri": "",
            "previewed_at": "2020-06-19"
        },
        "prices": {
            "usd": "0.18",
            "usd_foil": null,
            "usd_etched": null,
            "eur": "0.15",
            "eur_foil": null,
            "tix": null
        },
        "related_uris": {
            "gatherer": "https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=489604",
            "tcgplayer_infinite_articles": "https://infinite.tcgplayer.com/search?contentMode=article&game=magic&partner=scryfall&q=Long+Road+Home&utm_campaign=affiliate&utm_medium=api&utm_source=scryfall",
            "tcgplayer_infinite_decks": "https://infinite.tcgplayer.com/search?contentMode=deck&game=magic&partner=scryfall&q=Long+Road+Home&utm_campaign=affiliate&utm_medium=api&utm_source=scryfall",
            "edhrec": "https://edhrec.com/route/?cc=Long+Road+Home",
            "mtgtop8": "https://mtgtop8.com/search?MD_check=1&SB_check=1&cards=Long+Road+Home"
        },
        "purchase_uris": {
            "tcgplayer": "https://shop.tcgplayer.com/product/productsearch?id=216370&utm_campaign=affiliate&utm_medium=api&utm_source=scryfall",
            "cardmarket": "https://www.cardmarket.com/en/Magic/Products/Search?referrer=scryfall&searchString=Long+Road+Home&utm_campaign=card_prices&utm_medium=text&utm_source=scryfall",
            "cardhoarder": "https://www.cardhoarder.com/cards?affiliate_id=scryfall&data%5Bsearch%5D=Long+Road+Home&ref=card-profile&utm_campaign=affiliate&utm_medium=card&utm_source=scryfall"
        }
        */

        #endregion Scryfall data
    }
}