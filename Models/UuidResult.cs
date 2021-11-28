using Newtonsoft.Json;

namespace AcloudTools.Models
{
    class UuidResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("displayStatus")]
        public string DisplayStatus { get; set; }

        [JsonProperty("message")]
        public string Uuid { get; set; }
    }
}