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

        public int Need
        {
            get
            {
                return Want - (Count + FoilCount + EtchedCount);
            }
        }

        public int Trades
        {
            get
            {
                return (Count + FoilCount + EtchedCount) - Want;
            }
        }

        /// <summary>
        /// Number of foil cards wanted in collection
        /// </summary>
        public int WantFoil { get; set; }

        /// <summary>
        /// Number of etched foil cards wanted in collection
        /// </summary>
        public int WantEtched { get; set; }

        public Guid CardId { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser User { get; set; }

        /// <summary>
        /// The card in collection
        /// </summary>
        [Required]
        public Card Card { get; set; }

        /// <summary>
        /// If there are no cards in the collection and there are no wants, no need to store information about card in collection
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return Count + FoilCount + EtchedCount + Want + WantFoil + WantEtched > 0;
            }
        }

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