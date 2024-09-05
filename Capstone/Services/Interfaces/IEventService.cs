using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(Event eventModel, List<int> djIds, List<MemoryStream> imageStreams);
        Task<Event> GetEventByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> UpdateEventAsync(Event eventModel, List<int> djIds, List<MemoryStream> replaceImageStreams, List<MemoryStream> additionalImageStreams);
        Task<bool> DeleteEventAsync(int eventId);
    }
}
