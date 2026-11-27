using System.Text.Json.Serialization;

namespace Jellyfin.LiveTv.Listings.SchedulesDirectDtos
{
    /// <summary>
    /// The request body used to authenticate with Schedules Direct.
    /// </summary>
    public class TokenRequestDto
    {
        /// <summary>
        /// Gets or sets the account username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed account password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
