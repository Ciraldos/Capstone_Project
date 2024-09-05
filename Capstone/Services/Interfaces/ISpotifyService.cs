namespace Capstone.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<string> GetAccessTokenAsync();
    }
}
