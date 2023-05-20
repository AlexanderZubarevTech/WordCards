using CardWords.Core.Validations;
using System;

namespace CardWords.Core.Exceptions
{
    [Serializable]
    public class ValidationResultException : Exception
    {
        public ValidationResultException() { }

        public ValidationResultException(string message) : base(message) { }

        public ValidationResultException(ValidationResult validationResult) : this(validationResult.ToString()) 
        {
            ValidationResult = validationResult;
        }

        public ValidationResultException(string message, Exception inner) : base(message, inner) { }

        public ValidationResult ValidationResult { get; }

        protected ValidationResultException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
