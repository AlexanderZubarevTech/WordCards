using CardWords.Core.Ids;
using System;

namespace CardWords.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
        }

        public Entity(Id id)
        {
            Id = id;
        }

        public Entity(Id id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public Id Id { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
