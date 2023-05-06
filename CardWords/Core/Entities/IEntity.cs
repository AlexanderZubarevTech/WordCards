using System;

namespace CardWords.Core.Entities
{
    public interface IEntity
    {
        Id Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
