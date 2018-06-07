using System;
using System.Runtime.Serialization;

namespace Common.Foundation.ExceptionHandling.src.Exceptions
{
    public class ConflictApiException<T> : DomainApiResultException<T> where T : class
    {
        public ConflictApiException(string message, T data)
            : base(message, System.Net.HttpStatusCode.Conflict)
        {
            ExceptionDetails = data;
        }

        public ConflictApiException(string message, Exception innerException, T data)
            : base(message, innerException, System.Net.HttpStatusCode.Conflict)
        {
            ExceptionDetails = data;
        }

        protected ConflictApiException(SerializationInfo info, StreamingContext context, T data)
            : base(info, context, System.Net.HttpStatusCode.Conflict)
        {
            ExceptionDetails = data;
        }
    }
}
