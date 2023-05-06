using Microsoft.EntityFrameworkCore;

namespace CardWords.Core.Contexts
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
