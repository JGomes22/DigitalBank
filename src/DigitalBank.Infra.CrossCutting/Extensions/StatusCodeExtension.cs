using System.Net;

namespace DigitalBank.Infra.CrossCutting.Extensions
{
    public static class StatusCodeExtension
    {
        public static bool IsSuccess(this HttpStatusCode value)
        {
            int statusCode = (int)value;
            return statusCode >= 200 && statusCode < 300;
        }
    }
}
