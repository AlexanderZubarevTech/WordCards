using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WordCards.Core.Entities
{
    public interface IEntityMapping<TEntity>
        where TEntity : Entity
    {
        public static abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
