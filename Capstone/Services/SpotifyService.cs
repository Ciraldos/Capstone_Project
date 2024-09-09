using Capstone.Models.Spotify;
using Capstone.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Capstone.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SpotifySettings _spotifySettings;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public SpotifyService(IHttpClientFactory httpClientFactory, IOptions<SpotifySettings> spotifySettings)
        {
            _httpClientFactory = httpClientFactory;
            _spotifySettings = spotifySettings.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _tokenExpiration)
            {
                return _accessToken;
            }

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, _spotifySettings.TokenUrl);
            var clientIdSecret = $"{_spotifySettings.ClientId}:{_spotifySettings.ClientSecret}";
            var encodedClientIdSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientIdSecret));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedClientIdSecret);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<SpotifyAuthResponse>(json);

                _accessToken = tokenResponse.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                return _accessToken;
            }
            else
            {
                throw new Exception("Could not retrieve access token from Spotify");
            }
        }
    }
}
