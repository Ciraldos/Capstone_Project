using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class LocationService : ILocationService
    {
        private readonly DataContext _ctx;

        public LocationService(DataContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _ctx.Locations.OrderBy(l => l.LocationName).ToListAsync();
        }

        public async Task<Location> GetLocationByIdAsync(int locationId)
        {
            return await _ctx.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            _ctx.Locations.Add(location);
            await _ctx.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            var existingLocation = await _ctx.Locations.FindAsync(location.LocationId);

            if (existingLocation == null)
            {
                return null;
            }


            existingLocation.LocationName = location.LocationName;
            existingLocation.AddressGoogleApi = location.AddressGoogleApi;

            await _ctx.SaveChangesAsync();
            return existingLocation;
        }

        public async Task<bool> DeleteLocationAsync(int locationId)
        {
            var location = await _ctx.Locations.FindAsync(locationId);

            if (location == null)
            {
                return false;
            }

            _ctx.Locations.Remove(location);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
