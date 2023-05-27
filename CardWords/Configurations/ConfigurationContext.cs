using WordCards.Business.Languages;
using WordCards.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WordCards.Configurations.Contexts
{
    public sealed class ConfigurationContext : EntityContext
    {
        public ConfigurationContext() : base() 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<Configuration>(modelBuilder);
            EntityConfiguration<Language>(modelBuilder);
        }

        public DbSet<Configuration> Configurations { get; set; } = null!;

        public DbSet<Language> Languages { get; set; } = null!;
    }
}
