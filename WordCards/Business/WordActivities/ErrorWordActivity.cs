using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordCards.Business.LanguageWords;
using WordCards.Business.WordAction;
using WordCards.Core.Entities;

namespace WordCards.Business.WordActivities
{
    public sealed class ErrorWordActivity : Entity, IEntityMapping<ErrorWordActivity>
    {
        public static void Configure(EntityTypeBuilder<ErrorWordActivity> builder)
        {
            EntityMappingBuilder<ErrorWordActivity>.Create(builder)
                .Table("word_activity_errors", true, false)
                    .Column(x => x.LanguageWordId)
                    .End()
                    .Column(x => x.InfoId)
                    .End()
                    .Reference(x => x.LanguageWordId, x => x.Word, x => x.ErrorActivities)
                    .Reference(x => x.InfoId, x => x.Info, x => x.ErrorActivities)
                .End();
        }

        public ErrorWordActivity()
        {
        }

        public ErrorWordActivity(int languageWordId, int infoId)
        {
            LanguageWordId = languageWordId;
            InfoId = infoId;
        }

        public int LanguageWordId { get; set; }

        public LanguageWord Word { get; set; }

        public int InfoId { get; set; }

        public WordActionInfo Info { get; set; }
    }
}
