using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardWords.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CardWords.Core.Entities
{
    public sealed class EntityMappingBuilder<TEntity>
        where TEntity : Entity
    {
        public static EntityMappingBuilder<TEntity> Create(EntityTypeBuilder<TEntity> entityBuilder)
        {
            return new EntityMappingBuilder<TEntity>(entityBuilder);
        }

        private EntityMappingBuilder(EntityTypeBuilder<TEntity> entityBuilder)
        {
            this.entityBuilder = entityBuilder;
            properties = new List<string>();
        }

        private EntityTypeBuilder<TEntity> entityBuilder;

        private List<string> properties;

        public EntityMappingBuilder<TEntity> Table(string tableName, bool hasId = true, bool timestamp = true, bool isGeneratedId = true, bool idIsString = false)
        {
            entityBuilder.ToTable(tableName);

            AddIdColumn(hasId, isGeneratedId, idIsString);

            if (timestamp)
            {
                Column(x => x.Timestamp).End();
            }           

            return this;
        }        

        private void AddIdColumn(bool hasId, bool isGeneratedId, bool idIsString)
        {
            if (!hasId)
            {
                entityBuilder.Ignore(x => x.Id);

                return;
            }

            if (idIsString)
            {
                entityBuilder.Ignore(x => x.Id);

                entityBuilder.HasKey(x => x.IdAsString);

                var idStrPropBuilder = Column(x => x.IdAsString, "Id");

                idStrPropBuilder.ValueGeneratedNever();

                idStrPropBuilder.End();

                return;
            }

            entityBuilder.HasKey(x => x.Id);

            var idPropBuilder = Column(x => x.Id);

            if (isGeneratedId)
            {
                idPropBuilder.ValueGeneratedOnAdd();
            }

            idPropBuilder.End();
        }

        public PropertyMappingBuilder<TEntity, TProperty> Column<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpr, string? columnName = null)
        {
            var propBuilder = entityBuilder.Property(propertyExpr);

            properties.Add(propBuilder.Metadata.Name);

            if (!columnName.IsNullOrEmptyOrWhiteSpace())
            {
                propBuilder.HasColumnName(columnName);
            }
            
            return PropertyMappingBuilder<TEntity, TProperty>.Create(this, propBuilder);
        }

        public EntityMappingBuilder<TEntity> ReferenceList<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpr)            
        {
            var member = propertyExpr.Body as MemberExpression;

            if(member == null)
            {
                return this;
            }

            properties.Add(member.Member.Name);

            return this;
        }

        public EntityMappingBuilder<TEntity> HasKey(Expression<Func<TEntity, object?>> expr)
        {
            entityBuilder.HasKey(expr);

            return this;
        }

        public void End()
        {
            var ignoreProperties = typeof(TEntity).GetProperties()
                .Select(x => x.Name)
                .Except(properties)
                .ToList();

            if(ignoreProperties.Count > 0)
            {
                foreach( var property in ignoreProperties)
                {
                    entityBuilder.Ignore(property);
                }
            }
        }
    }
}
