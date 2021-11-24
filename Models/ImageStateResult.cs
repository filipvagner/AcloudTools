using Newtonsoft.Json;

namespace AcloudTools.Models
{
    class ImageStateResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("displayStatus")]
        public string DisplayStatus { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}