using System;
using System.Runtime.Serialization;

namespace Common.Foundation.ExceptionHandling
{
    /// <summary>
    /// Exception type for application exceptions
    /// </summary>
    [Serializable]
    public class DomainException<T> : Exception where T : class
    {
        public T ExceptionDetails { get; set; }

        public DomainException()
        { }

        public DomainException(string message, T details = null)
            : base(message)
        {
            ExceptionDetails = details;
        }

        public DomainException(string message, Exception innerException, T details = null)
            : base(message, innerException)
        {
            ExceptionDetails = details;
        }

        protected DomainException(SerializationInfo info, StreamingContext context, T details = null)
            : base(info, context)
        {
            ExceptionDetails = details;
        }
    }
}
