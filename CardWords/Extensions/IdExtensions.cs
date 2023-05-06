using Reminder.Core;
using Reminder.Core.Exceptions;
using System;

namespace Reminder.Extensions
{
    public static class IdExtensions
    {
        public static T As<T>(this Id id)
        {
            if(!Id.ExistType<T>())
            {
                throw new UnknownTypeException(nameof(T)); 
            }
            
            if(typeof(T) == typeof(string))
            {
                return (T) id.Value;
            }

            if(id.IsNotNumber)
            {
                throw new InvalidOperationException();
            }

            return (T) id.Value;
        }

        public static string AsString(this Id id) 
        {
            return As<string>(id);
        }

        public static bool HasValue(this Id? id)
        {
            return id.HasValue && id.Value.IsNotEmpty;
        }

    }
}
