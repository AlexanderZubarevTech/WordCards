using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using CardWords.Core.Ids;

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
                .End();
        }

        public WordActivity()
        {
        }

        public WordActivity(DateTime date, Id languageWordId, Id languageId, WordActivityType activityType)
        {
            Date = date;
            LanguageWordId = languageWordId;
            LanguageId = languageId;
            ActivityType = activityType;
        }

        public DateTime Date { get; set; }

        public Id LanguageWordId { get; set; }

        public Id LanguageId { get; set; }

        public WordActivityType ActivityType { get; set; }
    }
}
