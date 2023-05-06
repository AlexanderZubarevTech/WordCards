using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardWords.Core.Entities;
using System;

namespace CardWords.Core.ReadmeDeploy
{
    public sealed class ReadmeDeploy : Entity, IEntityMapping<ReadmeDeploy>
    {
        public static void Configure(EntityTypeBuilder<ReadmeDeploy> builder)
        {
            EntityMappingBuilder<ReadmeDeploy>.Create(builder)
                .Table("readme_deploys", true, true, false, true)
                .End();
        }

        public ReadmeDeploy()
        {            
        }

        public ReadmeDeploy(Id id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }
    }
}
