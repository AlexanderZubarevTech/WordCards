﻿using CardWords.Core;
using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardWords.Business.LanguageWords
{
    public sealed class LanguageWord : Entity, IEntityMapping<LanguageWord>
    {
        public static void Configure(EntityTypeBuilder<LanguageWord> builder)
        {
            EntityMappingBuilder<LanguageWord>.Create(builder)
                .Table("language_words")
                    .Column(x => x.LanguageId)
                    .End()
                    .Column(x => x.LanguageWordName)
                    .End()
                    .Column(x => x.Transcription)
                    .End()
                    .Column(x => x.Translation)
                    .End()
                .End();
        }

        public Id LanguageId { get; set; }

        public string LanguageWordName { get; set; }

        public string Transcription { get; set; }

        public string Translation { get; set; }
    }
}