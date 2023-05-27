using System;

namespace WordCards.Core.Entities
{
    public interface IEntity
    {
        int Id { get; set; }

        public string IdAsString { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
