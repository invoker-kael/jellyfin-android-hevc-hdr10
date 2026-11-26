using System.Text.Json;
using MediaBrowser.Model.Configuration;
using Xunit;

namespace Jellyfin.Model.Tests.Configuration
{
    public class ServerConfigurationTests
    {
        [Fact]
        public void EnableAndroidHdr10AutoDetection_IsEnabledByDefault()
        {
            Assert.True(new ServerConfiguration().EnableAndroidHdr10AutoDetection);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EnableAndroidHdr10AutoDetection_RoundTripsThroughJson(bool enabled)
        {
            var configuration = new ServerConfiguration
            {
                EnableAndroidHdr10AutoDetection = enabled
            };

            var json = JsonSerializer.Serialize(configuration);
            var deserialized = JsonSerializer.Deserialize<ServerConfiguration>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(enabled, deserialized.EnableAndroidHdr10AutoDetection);
        }
    }
}
