using Microsoft.AspNetCore.Identity;

namespace MagicCollectors.Core.Model
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<CollectionCard> CollectionCards { get; set; } = new List<CollectionCard>();

        public List<CollectionSet> CollectionSets { get; set; } = new List<CollectionSet>();
    }
}