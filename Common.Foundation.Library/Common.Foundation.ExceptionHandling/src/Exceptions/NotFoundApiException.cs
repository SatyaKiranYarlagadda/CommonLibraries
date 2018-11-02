using System;
using System.Runtime.Serialization;

namespace Common.Foundation.ExceptionHandling.Exceptions
{
    public class NotFoundApiException<T> : DomainApiResultException<T> where T : class
    {
        public NotFoundApiException(string message, T data)
            : base(message, System.Net.HttpStatusCode.NotFound)
        {
            ExceptionDetails = data;
        }

        public NotFoundApiException(string message, Exception innerException, T data)
            : base(message, innerException, System.Net.HttpStatusCode.NotFound)
        {
            ExceptionDetails = data;
        }

        protected NotFoundApiException(System.Runtime.Serialization.SerializationInfo info, StreamingContext context, T data)
            : base(info, context, System.Net.HttpStatusCode.NotFound)
        {
            ExceptionDetails = data;
        }
    }
}
