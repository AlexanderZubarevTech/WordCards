using WordCards.Business.LanguageWords;
using WordCards.Business.WordAction;
using WordCards.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace WordCards.Business.WordActivities
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
                    .Column(x => x.ActivityType)
                    .End()
                    .Column(x => x.InfoId)
                    .End()
                    .Reference(x => x.LanguageWordId, x => x.Word, x => x.Activities)
                    .Reference(x => x.InfoId, x => x.Info, x => x.Activities)
                .End();
        }

        public WordActivity()
        {
        }

        public WordActivity(DateTime date, int languageWordId, WordActivityType activityType, int infoId)
        {
            Date = date;
            LanguageWordId = languageWordId;
            ActivityType = activityType;
            InfoId = infoId;
        }

        public DateTime Date { get; set; }

        public int LanguageWordId { get; set; }

        public LanguageWord Word { get; set; }

        public WordActivityType ActivityType { get; set; }

        public int InfoId { get; set; }

        public WordActionInfo Info { get; set; }
    }
}
