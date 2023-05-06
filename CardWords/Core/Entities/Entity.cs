using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
        }

        public Id Id { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
