using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardWords.Core.Entities;

namespace CardWords.Configurations
{
    public sealed class Configuration : Entity, IEntityMapping<Configuration>
    {
        public static void Configure(EntityTypeBuilder<Configuration> builder)
        {
            EntityMappingBuilder<Configuration>.Create(builder)
                .Table("configurations", true, false, false, true)
                    .Column(x => x.Value)
                    .End()
                .End();
        }

        public Configuration()
        {
        }

        public new string Id
        {
            get
            {
                return IdAsString;
            }
            set
            {
                IdAsString = value;
            }
        }

        public string Value { get; set; }
    }
}
