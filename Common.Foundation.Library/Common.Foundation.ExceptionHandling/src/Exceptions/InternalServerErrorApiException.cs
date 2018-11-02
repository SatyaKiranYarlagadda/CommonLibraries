using System;
using System.Runtime.Serialization;

namespace Common.Foundation.ExceptionHandling.Exceptions
{
    public class InternalServerErrorApiException<T> : DomainApiResultException<T> where T : class
    {
        public InternalServerErrorApiException(string message, T data)
            : base(message, System.Net.HttpStatusCode.InternalServerError)
        {
            ExceptionDetails = data;
        }

        public InternalServerErrorApiException(string message, Exception innerException, T data)
            : base(message, innerException, System.Net.HttpStatusCode.InternalServerError)
        {
            ExceptionDetails = data;
        }

        protected InternalServerErrorApiException(SerializationInfo info, StreamingContext context, T data)
            : base(info, context, System.Net.HttpStatusCode.InternalServerError)
        {
            ExceptionDetails = data;
        }
    }
}
