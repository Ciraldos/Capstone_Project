using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Capstone.Services
{
    public class DjService : IDjService
    {
        private readonly DataContext _ctx;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SpotifyService _spotifyAuthSvc;
        public DjService(DataContext ctx, IHttpClientFactory httpClientFactory, SpotifyService spotifyAuthService)
        {
            _ctx = ctx;
            _httpClientFactory = httpClientFactory;
            _spotifyAuthSvc = spotifyAuthService;
        }

        public async Task<Dj> CreateDjAsync(Dj dj)
        {
            if (dj == null)
            {
                throw new ArgumentNullException(nameof(dj));
            }

            // Ottieni l'URL dell'immagine dall'API di Spotify
            var imageUrl = await GetSpotifyArtistImageUrlAsync(dj.ArtistSpotifyId);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                dj.Img = imageUrl;
            }
            else
            {
                throw new Exception("Could not retrieve artist image from Spotify");
            }

            _ctx.Djs.Add(dj);
            await _ctx.SaveChangesAsync();

            return dj;
        }

        private async Task<string> GetSpotifyArtistImageUrlAsync(string artistSpotifyId)
        {
            var accessToken = await _spotifyAuthSvc.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var endpoint = $"https://api.spotify.com/v1/artists/{artistSpotifyId}";

            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("images", out JsonElement imagesElement) && imagesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var image in imagesElement.EnumerateArray())
                        {
                            if (image.TryGetProperty("url", out JsonElement urlElement))
                            {
                                var url = urlElement.GetString();
                                if (!string.IsNullOrEmpty(url))
                                {
                                    return url;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Spotify API request failed with status code {response.StatusCode}");
            }

            return null;
        }

        public async Task<List<Dj>> GetAllDjAsync()
        {
            return await _ctx.Djs.Include(d => d.Events).ToListAsync();
        }

        public async Task<Dj> GetDjByIdAsync(int id)
        {
            return await _ctx.Djs.Include(d => d.Events).Where(d => d.DjId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteDjAsync(int id)
        {
            var dj = await _ctx.Djs.Where(d => d.DjId == id).FirstOrDefaultAsync();
            if (dj == null) return false;
            _ctx.Remove(dj);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Dj> UpdateDjAsync(Dj dj)
        {
            var existingDj = await _ctx.Djs.FirstOrDefaultAsync(d => d.DjId == dj.DjId);

            if (existingDj == null)
            {
                throw new KeyNotFoundException("DJ not found");
            }

            existingDj.ArtistName = dj.ArtistName;
            existingDj.ArtistSpotifyId = dj.ArtistSpotifyId;
            existingDj.ArtistDescription = dj.ArtistDescription;
            existingDj.Img = await GetSpotifyArtistImageUrlAsync(existingDj.ArtistSpotifyId);

            await _ctx.SaveChangesAsync();

            return existingDj;
        }

        public async Task<List<Event>> GetRelatedEventsAsync(int djId)
        {
            // Ottieni il DJ con gli eventi e i generi associati
            var dj = await _ctx.Djs
            .Include(d => d.Events)
                .ThenInclude(e => e.EventImgs)
            .Include(d => d.Events)
                .ThenInclude(e => e.Location)
            .Include(d => d.Events)
                .ThenInclude(e => e.TicketTypes)
            .Include(d => d.Events)
                .ThenInclude(e => e.Genres)
            .FirstOrDefaultAsync(d => d.DjId == djId);

            if (dj == null)
            {
                throw new KeyNotFoundException("DJ not found");
            }

            // Trova i generi degli eventi dell'artista
            var genreIds = dj.Events
                .SelectMany(e => e.Genres)
                .Select(g => g.GenreId)
                .Distinct()
                .ToList();

            if (!genreIds.Any())
            {
                return new List<Event>(); // Nessun genere trovato, ritorna una lista vuota
            }

            // Trova gli eventi con gli stessi generi, escludendo quelli già associati al DJ
            var relatedEvents = await _ctx.Events
                .Where(e => e.Genres.Any(g => genreIds.Contains(g.GenreId)))
                .Where(e => !dj.Events.Select(evt => evt.EventId).Contains(e.EventId))
                .Include(e => e.EventImgs) // Include immagini evento
                .Include(e => e.Location) // Include location
                .Include(e => e.TicketTypes) // Include tipi di biglietti
                .ToListAsync();

            return relatedEvents;
        }


        public async Task<List<Dj>> SearchDjsAsync(string name)
        {
            return await _ctx.Djs
                .Where(d => d.ArtistName.Contains(name))
                .ToListAsync();
        }
    }
}
