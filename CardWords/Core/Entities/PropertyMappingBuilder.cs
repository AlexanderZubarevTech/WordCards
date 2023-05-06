using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CardWords.Core.Entities
{
    public sealed class PropertyMappingBuilder<TEntity, TProperty>
        where TEntity : Entity
    {
        public static PropertyMappingBuilder<TEntity, TProperty> Create(EntityMappingBuilder<TEntity> mappingBuilder,
            PropertyBuilder<TProperty> propertyBuilder)
        {
            return new PropertyMappingBuilder<TEntity, TProperty>(mappingBuilder, propertyBuilder);
        }

        private PropertyMappingBuilder(EntityMappingBuilder<TEntity> mappingBuilder, PropertyBuilder<TProperty> propertyBuilder)
        {
            parent = mappingBuilder;
            this.propertyBuilder = propertyBuilder;            
        }

        private EntityMappingBuilder<TEntity> parent;
        private PropertyBuilder<TProperty> propertyBuilder;        

        public PropertyMappingBuilder<TEntity, TProperty> IsRequired()
        {
            propertyBuilder.IsRequired();

            return this;
        }

        public PropertyMappingBuilder<TEntity, TProperty> ValueGeneratedOnAdd()
        {
            propertyBuilder.ValueGeneratedOnAdd();

            return this;
        }

        public PropertyMappingBuilder<TEntity, TProperty> ValueGeneratedNever()
        {
            propertyBuilder.ValueGeneratedNever();

            return this;
        }

        public PropertyMappingBuilder<TEntity, TProperty> HasColumnType(string? typeName)
        {
            propertyBuilder.HasColumnType(typeName);

            return this;
        }

        public PropertyMappingBuilder<TEntity, TProperty> IsConcurrencyToken(bool concurrencyToken = true)
        {
            propertyBuilder.IsConcurrencyToken(concurrencyToken);

            return this;
        }

        public PropertyMappingBuilder<TEntity, TProperty> HasConversion(ValueConverter converter)
        {
            propertyBuilder.HasConversion(converter);

            return this;
        }

        public EntityMappingBuilder<TEntity> End()
        {
            return parent;
        }
    }
}
