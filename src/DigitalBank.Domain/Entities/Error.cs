using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DigitalBank.Domain.Entities
{
    public class Error
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "instance")]
        public string Instance { get; set; }
        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; }


        public Error() => Extensions = new Dictionary<string, object>();
        public Error(string errorMessage)
        {
            Extensions = new Dictionary<string, object>();
            ErrorMessage = errorMessage;
        }
        public Error(string errorMessage, ExecutionContext context, HttpRequest req)
        {
            Extensions = new Dictionary<string, object>();
            ErrorMessage = errorMessage;
            Instance = req?.HttpContext?.Request?.Path;
            Extensions.Add("invocationId", context?.InvocationId);
        }
        public Error(string propertyName, string errorMessage)
        {
            Extensions = new Dictionary<string, object>();
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
        public Error(string propertyName, string errorMessage, ExecutionContext context, HttpRequest req)
        {
            Extensions = new Dictionary<string, object>();
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            Instance = req?.HttpContext?.Request?.Path;
            Extensions.Add("invocationId", context?.InvocationId);
        }
        public static IList<Error> GenerateFailure(string errorMessage) => new List<Error> { new Error(errorMessage) };

    }
}
