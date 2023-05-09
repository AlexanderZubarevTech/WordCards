using System;

namespace CardWords.Core.Entities
{
    public interface IEntity
    {
        int Id { get; set; }

        public string IdAsString { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
