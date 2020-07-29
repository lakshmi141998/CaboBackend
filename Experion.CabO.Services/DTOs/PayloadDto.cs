using Newtonsoft.Json;

namespace Experion.CabO.Services.DTOs
{
    public class Payload
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
    }

    public class TokenDto
    {
        public string token { get; set; }
    }
}
