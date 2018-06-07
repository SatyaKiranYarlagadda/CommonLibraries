using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Foundation.ResponseObjects
{
    public class ApiSuccessResponse<T> : IResult<T>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseStatusCode Status { get; set; } = ResponseStatusCode.Success;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; } = null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Message { get; set; }
    }
}
