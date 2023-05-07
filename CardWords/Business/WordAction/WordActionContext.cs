using CardWords.Business.LanguageWords;
using CardWords.Business.WordActivities;
using CardWords.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CardWords.Business.WordAction
{
    public sealed class WordActionContext : EntityContext
    {
        public WordActionContext() : base() 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<LanguageWord>(modelBuilder);            
            EntityConfiguration<WordActivity>(modelBuilder);
        }


        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;

        public DbSet<WordActivity> WordActivities { get; set; } = null!;
    }
}
