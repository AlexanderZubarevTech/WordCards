using CardWords.Core.Entities;
using CardWords.Core.Exceptions;
using CardWords.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CardWords.Core.Validations
{

    public sealed class ValidationResult : IEnumerable
    {
        public ValidationResult() 
        {
            errors = new List<ErrorMessage>();
        }

        private List<ErrorMessage> errors;

        public int ErrorCount => errors.Count;

        public bool HasError => ErrorCount > 0;

        public IEnumerator<ErrorMessage> GetEnumerator() => errors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => errors.GetEnumerator();

        public void Add(string message)
        {
            if(!message.IsNullOrEmpty())
            {
                errors.Add(ErrorMessage.Create(message));
            }
        }

        public void AddRequired<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expr)
            where TEntity : Entity
        {
            var message = ErrorMessage.CreateField("Поле \"{0}\" обязательно для заполнения", expr);

            errors.Add(message);
        }        

        public void AddGreaterThan<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expr, TProperty value)
            where TEntity : Entity
        {
            var message = ErrorMessage.CreateField($"Поле \"{{{0}}}\" должно быть больше {value}", expr);

            errors.Add(message);
        }

        public void AddLessThan<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expr, TProperty value)
            where TEntity : Entity
        {
            var message = ErrorMessage.CreateField($"Поле {{{0}}} должно быть меньше {value}", expr);

            errors.Add(message);
        }

        public void ThrowIfHasError()
        {
            if(HasError)
            {
                throw new ValidationResultException(this);
            }
        }

        public static void ThrowError(string message)
        {
            throw new ValidationResultException(message);
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            foreach (var error in errors)
            {
                str.Append("\n");
                str.Append(error.ToString());
            }

            return str.ToString();
        }

        
    }
}
