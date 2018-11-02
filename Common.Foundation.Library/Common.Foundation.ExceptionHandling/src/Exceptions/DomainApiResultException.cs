using System;
using System.Net;
using System.Runtime.Serialization;

namespace Common.Foundation.ExceptionHandling.Exceptions
{
    public class DomainApiResultException<T> : DomainException<T> where T : class
    {
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.BadRequest;        

        public DomainApiResultException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, T details = null)
            : base(message, details)
        {
            HttpStatusCode = httpStatusCode;
        }

        public DomainApiResultException(string message, Exception innerException, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, T details = null)
            : base(message, innerException, details)
        {
            HttpStatusCode = httpStatusCode;
        }

        protected DomainApiResultException(SerializationInfo info, StreamingContext context, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, T details = null)
            : base(info, context, details)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
