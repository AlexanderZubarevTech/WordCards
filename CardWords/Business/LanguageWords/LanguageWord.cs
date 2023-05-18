using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

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
                    .Column(x => x.TranslationLanguageId)
                    .End()
                    .Column(x => x.LanguageWordName)
                    .End()
                    .Column(x => x.Transcription)
                    .End()
                    .Column(x => x.Translation)
                    .End()
                .End();
        }

        public LanguageWord()
        {
        }

        public LanguageWord(EditLanguageWord editWord, DateTime now)
        {
            Id = editWord.Id;
            Timestamp = now;
            LanguageId = editWord.LanguageId;
            TranslationLanguageId = editWord.TranslationLanguageId;
            LanguageWordName = editWord.LanguageWordName;
            Transcription = editWord.Transcription;
            Translation = editWord.Translation;
        }

        public int LanguageId { get; set; }

        public int TranslationLanguageId { get; set; }

        public string LanguageWordName { get; set; }

        public string Transcription { get; set; }

        public string Translation { get; set; }
    }
}
