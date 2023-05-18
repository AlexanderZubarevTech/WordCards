using CardWords.Business.Languages;
using CardWords.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CardWords.Business.LanguageWords
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
        }


        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;     
        
        public DbSet<Language> Languages { get; set; } = null!;
    }
}
