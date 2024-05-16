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
        /// Number of etched foil cards in collection
        /// </summary>
        public int EtchedCount { get; set; }

        /// <summary>
        /// Number of normal cards wanted in collection
        /// </summary>
        public int Want { get; set; }

        /// <summary>
        /// Number of foil cards wanted in collection
        /// </summary>
        public int WantFoil { get; set; }

        /// <summary>
        /// Number of etched foil cards wanted in collection
        /// </summary>
        public int WantEtched { get; set; }

        public Guid CardId { get; set; }

        /// <summary>
        /// The card in collection
        /// </summary>
        [Required]
        public Card Card { get; set; }

        public void Load(CollectionCard card)
        {
            Count = card.Count;
            FoilCount = card.FoilCount;
            EtchedCount = card.EtchedCount;
            Want = card.Want;
            WantFoil = card.WantFoil;
            WantEtched = card.WantEtched;
        }
    }
}