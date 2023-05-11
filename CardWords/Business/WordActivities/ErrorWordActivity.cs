using CardWords.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardWords.Business.WordActivities
{
    public sealed class ErrorWordActivity : Entity, IEntityMapping<ErrorWordActivity>
    {
        public static void Configure(EntityTypeBuilder<ErrorWordActivity> builder)
        {
            EntityMappingBuilder<ErrorWordActivity>.Create(builder)
                .Table("word_activity_errors", true, false)                    
                    .Column(x => x.LanguageWordId)
                    .End()
                    .Column(x => x.LanguageId)
                    .End()                    
                    .Column(x => x.InfoId)
                    .End()
                .End();
        }

        public ErrorWordActivity()
        {
        }

        public ErrorWordActivity(int languageWordId, int languageId, int infoId)
        {            
            LanguageWordId = languageWordId;
            LanguageId = languageId;           
            InfoId = infoId;
        }        

        public int LanguageWordId { get; set; }

        public int LanguageId { get; set; }        

        public int InfoId { get; set; }
    }
}
