using CardWords.Core;
using CardWords.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CardWords.Configurations.Contexts
{
    public sealed class ConfigurationContext : EntityContext
    {
        public ConfigurationContext() : base() 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<Configuration>(modelBuilder);            
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Id>()
                .HaveConversion<Id.IdStringConverter>();
        }

        public DbSet<Configuration> Configurations { get; set; } = null!;        
    }
}
