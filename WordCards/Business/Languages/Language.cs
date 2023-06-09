﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordCards.Core.Entities;

namespace WordCards.Business.Languages
{
    public sealed class Language : Entity, IEntityMapping<Language>
    {
        public static void Configure(EntityTypeBuilder<Language> builder)
        {
            EntityMappingBuilder<Language>.Create(builder)
                .Table("languages")
                    .Column(x => x.Name)
                    .End()
                .End();
        }

        public Language()
        {
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
