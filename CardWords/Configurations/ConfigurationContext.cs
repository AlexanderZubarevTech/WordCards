﻿using CardWords.Core.Contexts;
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

        public DbSet<Configuration> Configurations { get; set; } = null!;        
    }
}