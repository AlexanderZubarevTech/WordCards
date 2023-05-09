using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CardWords.Business.WordActivities
{
    public sealed class WordActivity : Entity, IEntityMapping<WordActivity>
    {
        public static void Configure(EntityTypeBuilder<WordActivity> builder)
        {
            EntityMappingBuilder<WordActivity>.Create(builder)
                .Table("word_activities", true, false)
                    .Column(x => x.Date)
                    .End()
                    .Column(x => x.LanguageWordId)
                    .End()
                    .Column(x => x.LanguageId)
                    .End()
                    .Column(x => x.ActivityType)
                    .End()
                    .Column(x => x.InfoId)
                    .End()
                .End();
        }

        public WordActivity()
        {
        }

        public WordActivity(DateTime date, int languageWordId, int languageId, WordActivityType activityType, int infoId)
        {
            Date = date;
            LanguageWordId = languageWordId;
            LanguageId = languageId;
            ActivityType = activityType;
            InfoId = infoId;
        }

        public DateTime Date { get; set; }

        public int LanguageWordId { get; set; }

        public int LanguageId { get; set; }

        public WordActivityType ActivityType { get; set; }

        public int InfoId { get; set; }
    }
}
