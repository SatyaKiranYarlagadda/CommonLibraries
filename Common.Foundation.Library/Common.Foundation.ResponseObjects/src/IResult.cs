using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Foundation.ResponseObjects
{
    public interface IResult<T>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        ResponseStatusCode Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        T Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string[] Message { get; set; }
    }
}
