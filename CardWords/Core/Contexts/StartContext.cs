using Microsoft.EntityFrameworkCore;

namespace WordCards.Core.Contexts
{
    public sealed class StartContext : BaseContext
    {
        public StartContext() : base()
        {
            //Database.CanConnect();
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<ReadmeDeploy.ReadmeDeploy>(modelBuilder);
        }

        public DbSet<ReadmeDeploy.ReadmeDeploy> Deploys { get; set; } = null!;
    }
}
