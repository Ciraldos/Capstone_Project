using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<Location> GetLocationByIdAsync(int locationId);
        Task<Location> CreateLocationAsync(Location location);
        Task<Location> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int locationId);
    }
}
