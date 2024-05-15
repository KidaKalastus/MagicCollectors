using System.ComponentModel.DataAnnotations;

namespace MagicCollectors.Core.Model
{
    public class CollectionCard
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Number of normal cards in collection
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Number of foil cards in collection
        /// </summary>
        public int FoilCount { get; set; }

        /// <summary>
        /// Number of normal cards wanted in collection
        /// </summary>
        public int Want { get; set; }

        /// <summary>
        /// Number of foil cards wanted in collection
        /// </summary>
        public int WantFoil { get; set; }

        public Guid CardId { get; set; }

        /// <summary>
        /// The card information is related to
        /// </summary>
        [Required]
        public Card Card { get; set; }

        public void Load(CollectionCard card)
        {
            Count = card.Count;
            FoilCount = card.FoilCount;
            Want = card.Want;
            WantFoil = card.WantFoil;
        }
    }
}