using WordCards.Core.Entities;
using System.Linq.Expressions;
using System;

namespace WordCards.Core.Validations
{
    public sealed class ErrorMessage
    {
        public static ErrorMessage Create(string message)
        {
            return new ErrorMessage(message);
        }

        public static ErrorMessage CreateField<TEntity, TProperty>(string message, Expression<Func<TEntity, TProperty>> expr)
            where TEntity : Entity
        {
            var propName = (expr.Body as MemberExpression).Member.Name;

            return new ErrorMessage(message, true, propName);
        }

        private ErrorMessage(string message) : this(message, false, null)
        {
        }

        private ErrorMessage(string message, bool hasField, string? propertyName)
        {
            Message = message;
            HasField = hasField;
            PropertyName = propertyName;
        }

        public string Message { get; }

        public bool HasField { get; }

        public string? PropertyName { get; }

        public string GetMessage(string? fieldViewName)
        {
            if(HasField)
            {
                return string.Format(Message, fieldViewName);
            }

            return Message;
        }

        public override string ToString()
        {
            return GetMessage(PropertyName);
        }
    }
}
