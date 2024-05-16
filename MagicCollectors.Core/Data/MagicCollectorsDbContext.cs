using MagicCollectors.Core.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MagicCollectors.Core.Data
{
    public class MagicCollectorsDbContext : IdentityDbContext<ApplicationUser>
    {
        public MagicCollectorsDbContext(DbContextOptions<MagicCollectorsDbContext> options) : base(options)
        {
        }

        public MagicCollectorsDbContext() : base()
        {
        }

        public DbSet<Set> Sets { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<PromoType> PromoTypes { get; set; }

        public DbSet<FrameEffect> FrameEffects { get; set; }

        public DbSet<CollectionSet> CollectionSets { get; set; }

        public DbSet<CollectionCard> CollectionCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\Auwau;Database=MagicCollectors;Trusted_Connection=True");
            base.OnConfiguring(optionsBuilder);
        }
    }
}