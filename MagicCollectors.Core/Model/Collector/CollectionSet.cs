using System.ComponentModel.DataAnnotations;

namespace MagicCollectors.Core.Model
{
    public class CollectionSet
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Number of normal cards in collection
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Number of normal cards wanted in collection
        /// </summary>
        public int Want { get; set; }

        /// <summary>
        /// Number of cards missing
        /// Be aware that Missing != Want - Count as there may be some cards in collection the user doesn't want
        /// i.e. A user may have 8 of one card and only want 4. Then 0 cards are missing while the above
        /// calculation would result in -4 missing cards
        /// </summary>
        public int Missing { get; set; }

        /// <summary>
        /// Value of all cards in the set
        /// </summary>
        public decimal ValueOfOwnedCards { get; set; }

        /// <summary>
        /// Cost of cards that collector wants but doesn't have
        /// </summary>
        public decimal CostOfMissingCards { get; set; }

        public Guid SetId { get; set; }

        /// <summary>
        /// The card information is related to
        /// </summary>
        [Required]
        public Set Set { get; set; }

        public void Load(CollectionSet set)
        {
            Count = set.Count;
            Want = set.Want;
            Missing = set.Missing;
            ValueOfOwnedCards = set.ValueOfOwnedCards;
            CostOfMissingCards = set.CostOfMissingCards;
        }

        public bool SetHasChanged(CollectionSet set)
        {
            if (set.Count != Count)
            {
                return true;
            }

            if (set.Want != Want)
            {
                return true;
            }

            if (set.Missing != Missing)
            {
                return true;
            }

            if (set.ValueOfOwnedCards != ValueOfOwnedCards)
            {
                return true;
            }

            if (set.CostOfMissingCards != CostOfMissingCards)
            {
                return true;
            }

            return false;
        }
    }
}