using System.ComponentModel.DataAnnotations;

namespace MagicCollectors.Core.Model
{
    /// <summary>
    /// A Set object represents a group of related Magic cards. All Card objects on Scryfall belong to exactly one set.
    /// Due to Magic’s long and complicated history, many un-official sets are included as a way to group promotional or outlier cards together.
    /// Such sets will likely have a code that begins with p or t, such as pcel or tori.
    /// Official sets always have a three-letter set code, such as zen.
    /// </summary>
    public class Set
    {
        /// <summary>
        /// A unique ID for this set on Scryfall that will not change.
        /// Scryfall: id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The unique three to five-letter code for this set
        /// Scryfall: code
        /// </summary>
        [Required]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The English name of the set.
        /// Scryfall: name
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A computer-readable classification for this set
        /// Scryfall: set_type
        /// </summary>
        public SetType Type { get; set; } = SetType.undefined;

        /// <summary>
        /// The date the set was released or the first card was printed in the set (in GMT-8 Pacific time).
        /// Scryfall: released_at
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// The block code for this set, if any.
        /// Scryfall: block_code
        /// </summary>
        public string? BlockCode { get; set; }

        /// <summary>
        /// The block or group name code for this set, if any.
        /// Scryfall: block
        /// </summary>
        public string? Block { get; set; }

        /// <summary>
        /// The set code for the parent set, if any. promo and token sets often have a parent set.
        /// Scryfall: parent_set_code
        /// </summary>
        public string? ParentSetCode { get; set; }

        /// <summary>
        /// The number of cards in this set.
        /// Scryfall: card_count
        /// </summary>
        public int CardCount { get; set; } = 0;

        /// <summary>
        /// True if this set was only released in a video game.
        /// Scryfall: digital
        /// </summary>
        public bool Digital { get; set; } = false;

        /// <summary>
        /// True if this set contains only foil cards.
        /// Scryfall: foil_only
        /// </summary>
        public bool FoilOnly { get; set; } = false;

        /// <summary>
        /// True if this set contains only nonfoil cards.
        /// Scryfall: nonfoil_only
        /// </summary>
        public bool NonFoilOnly { get; set; } = false;

        /// <summary>
        /// A URI to an SVG file for this set’s icon on Scryfall’s CDN. Hotlinking this image isn’t recommended,
        /// because it may change slightly over time. You should download it and use it locally for your particular
        /// user interface needs.
        /// Scryfall: icon_svg_uri
        /// </summary>
        public string? IconSvgUri { get; set; }

        /// <summary>
        /// A list of all the cards in the set
        /// </summary>
        public List<Card> Cards { get; set; } = new List<Card>();

        public List<CollectionSet> CollectionSets { get; set; } = new List<CollectionSet>();

        /*
        /// <summary>
        /// The unique code for this set on MTGO, which may differ from the regular code.
        /// Scryfall: mtgo_code
        /// </summary>
        public string MtgoCode { get; set; }

        /// <summary>
        /// This set’s ID on TCGplayer’s API, also known as the groupId.
        /// Scryfall: tcgplayer_id
        /// </summary>
        public int TcgPlayerId { get; set; }

        /// <summary>
        /// The denominator for the set’s printed collector numbers.
        /// Scryfall: printed_size
        /// </summary>
        public int PrintedSize { get; set; }

        /// <summary>
        /// A link to this set’s permapage on Scryfall’s website.
        /// Scryfall: scryfall_uri
        /// </summary>
        public string ScryfallUri { get; set; }

        /// <summary>
        /// A link to this set object on Scryfall’s API.
        /// Scryfall: uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// A Scryfall API URI that you can request to begin paginating over the cards in this set.
        /// Scryfall: search_uri
        /// </summary>
        public string SearchUri { get; set; }
        */
    }
}