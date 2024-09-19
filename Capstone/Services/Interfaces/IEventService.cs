using Capstone.Models;
using Capstone.Models.ViewModels;

namespace Capstone.Services.Interfaces
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(Event eventModel, List<int> djIds, List<int> SelectedGenres, List<IFormFile> imageStreams, List<int> ticketTypesId, List<int> ticketQuantities);
        Task<Event> GetEventByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<List<Event>> GetPopularEventsAsync();
        Task<Event> UpdateEventAsync(Event eventModel, List<int> djIds, List<int> selectedGenres, List<string> replaceImagePaths, List<string> additionalImagePaths, List<int> ticketTypesId);
        Task<bool> DeleteEventAsync(int eventId);
        Task<List<EventViewModel>> SearchEventsAsync(string query);

    }
}
