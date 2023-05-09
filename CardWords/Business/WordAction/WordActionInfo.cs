using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

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
                .End();
        }

        public WordActionInfo() 
        {
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MaxSequence { get; set; }

        public int WordsCount { get; set; }

        public int NewWordsCount { get; set; }

        public int CorrectAnswersCount { get; set; }

        public int WrongAnswersCount { get; set; }

        public int SelectedCardWordsCount { get; set; }

        public TimeSpan Duration => EndDate - StartDate;
    }
}
