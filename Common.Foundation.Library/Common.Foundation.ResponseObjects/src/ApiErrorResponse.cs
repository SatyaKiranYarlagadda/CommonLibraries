using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Foundation.ResponseObjects
{
    public class ApiErrorResponse<T> : IResult<T>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseStatusCode Status { get; set; } = ResponseStatusCode.Error;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Message { get; set; }
    }
}
