using CardWords.Business.WordActivities;
using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace CardWords.Business.WordAction
{
    public sealed class WordActionInfo : Entity, IEntityMapping<WordActionInfo>
    {
        public static void Configure(EntityTypeBuilder<WordActionInfo> builder)
        {
            EntityMappingBuilder<WordActionInfo>.Create(builder)
                .Table("word_actions", true, false)
                    .Column(x => x.StartDate)
                    .End()
                    .Column(x => x.EndDate)
                    .End()
                    .Column(x => x.LanguageId)
                    .End()
                    .Column(x => x.TranslationLanguageId)
                    .End()
                    .Column(x => x.MaxSequence)
                    .End()
                    .Column(x => x.WordsCount)
                    .End()
                    .Column(x => x.NewWordsCount)
                    .End()
                    .Column(x => x.CorrectAnswersCount)
                    .End()
                    .Column(x => x.WrongAnswersCount)
                    .End()
                    .Column(x => x.SelectedCardWordsCount)
                    .End()
                    .ReferenceList(x => x.Activities)
                    .ReferenceList(x => x.ErrorActivities)
                .End();
        }

        public WordActionInfo() 
        {
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LanguageId { get; set; }

        public int TranslationLanguageId { get; set; }

        public int MaxSequence { get; set; }

        public int WordsCount { get; set; }

        public int NewWordsCount { get; set; }

        public int CorrectAnswersCount { get; set; }

        public int WrongAnswersCount { get; set; }

        public int SelectedCardWordsCount { get; set; }

        public List<WordActivity> Activities { get; set; }

        public List<ErrorWordActivity> ErrorActivities { get; set; }

        public TimeSpan Duration => EndDate - StartDate;
    }
}
