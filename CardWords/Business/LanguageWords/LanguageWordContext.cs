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
        }


        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;        
    }
}
