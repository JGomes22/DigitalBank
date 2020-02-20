using System.Collections.Generic;
using System.Net;

namespace DigitalBank.Domain.Entities
{
    public class HttpResult
    {
        public HttpStatusCode StatusCode { get; private set; }
        public IList<Error> Errors { get; private set; }

        public HttpResult(HttpStatusCode statusCode, IList<Error> errors)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }

    public class HttpResult<T> : HttpResult
    {
        public T Value { get; set; }

        public HttpResult(T value, HttpStatusCode statusCode, IList<Error> errors)
            : base(statusCode, errors)
        {
            Value = value;
        }
    }
}
