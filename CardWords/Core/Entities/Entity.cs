using System;

namespace WordCards.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
        }

        public Entity(int id)
        {
            Id = id;
        }

        public Entity(int id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public int Id { get; set; }

        public string IdAsString { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
