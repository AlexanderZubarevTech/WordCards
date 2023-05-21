﻿using CardWords.Business.Languages;
using CardWords.Business.WordActivities;
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
            EntityConfiguration<WordActivity>(modelBuilder);
        }

        public DbSet<LanguageWord> LanguageWords { get; set; } = null!;     
        
        public DbSet<Language> Languages { get; set; } = null!;

        public DbSet<WordActivity> WordActivities { get; set; } = null!;
    }
}
