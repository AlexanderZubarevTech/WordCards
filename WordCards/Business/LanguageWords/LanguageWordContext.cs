using WordCards.Business.Languages;
using WordCards.Business.WordActivities;
using WordCards.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WordCards.Business.LanguageWords
{
    public sealed class LanguageWordContext : EntityContext
    {
        public LanguageWordContext() : base() 
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<LanguageWord>(modelBuilder);
            EntityConfiguration<Language>(modelBuilder);
            EntityConfiguration<WordActivity>(modelBuilder);
        }

        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;     
        
        public DbSet<Language> Languages { get; set; } = null!;

        public DbSet<WordActivity> WordActivities { get; set; } = null!;
    }
}
