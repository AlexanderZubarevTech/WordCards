using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reminder.Core.Entities
{
    public interface IEntityMapping<TEntity>
        where TEntity : Entity
    {
        public static abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
