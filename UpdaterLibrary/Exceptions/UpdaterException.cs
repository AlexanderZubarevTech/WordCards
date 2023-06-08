using System;

namespace UpdaterLibrary.Exceptions
{
    [Serializable]
    public class UpdaterException : Exception
    {
        public UpdaterException() { }

        public UpdaterException(string message) : base(message) { }

        public UpdaterException(string message, Exception inner) : base(message, inner) { }

        protected UpdaterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
