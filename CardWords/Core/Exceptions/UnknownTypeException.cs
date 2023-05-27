using System;

namespace WordCards.Core.Exceptions
{
    [Serializable]
    public class UnknownTypeException : Exception
    {
        public UnknownTypeException() { }

        public UnknownTypeException(string message) : base(message) { }

        public UnknownTypeException(Type unknownType) : base($"Using unknown type: {unknownType}") { }

        public UnknownTypeException(string message, Exception inner) : base(message, inner) { }

        protected UnknownTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
