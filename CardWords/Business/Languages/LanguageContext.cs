using CardWords.Business.LanguageWords;
using CardWords.Business.WordAction;
using CardWords.Business.WordActivities;
using CardWords.Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CardWords.Business.Languages
{
    public sealed class LanguageContext : EntityContext
    {
        public LanguageContext() : base() 
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<LanguageWord>(modelBuilder);
            EntityConfiguration<Language>(modelBuilder);
            EntityConfiguration<WordActivity>(modelBuilder);
            EntityConfiguration<ErrorWordActivity>(modelBuilder);
            EntityConfiguration<WordActionInfo>(modelBuilder);
        }


        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;     
        
        public DbSet<Language> Languages { get; set; } = null!;        

        public DbSet<WordActivity> WordActivities { get; set; } = null!;

        public DbSet<ErrorWordActivity> ErrorWordActivities { get; set; } = null!;

        public DbSet<WordActionInfo> WordActionInfos { get; set; } = null!;
    }
}
