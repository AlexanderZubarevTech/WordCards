using Microsoft.EntityFrameworkCore;
using WordCards.Core.Entities;
using System.Configuration;

namespace WordCards.Core.Contexts
{
    public class BaseContext : DbContext
    {
        public BaseContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        public static void EntityConfiguration<TEntity>(ModelBuilder modelBuilder)
            where TEntity : Entity, IEntityMapping<TEntity>
        {
            modelBuilder.Entity<TEntity>(TEntity.Configure);
        }
    }
}
