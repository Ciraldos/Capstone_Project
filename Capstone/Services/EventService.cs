using Capstone.Context;
using Capstone.Helpers;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class EventService : IEventService
    {
        private readonly DataContext _ctx;

        public EventService(DataContext context)
        {
            _ctx = context;
        }
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _ctx.Events
                .Include(e => e.Location)
                .Include(e => e.Djs)
                .Include(e => e.EventImgs)
                .ToListAsync();
        }


        public async Task<Event> CreateEventAsync(Event eventModel, List<int> djIds, List<MemoryStream> imageStreams, List<int> ticketTypesId, List<int> ticketQuantities)
        {
            var existingLocation = await _ctx.Locations.FindAsync(eventModel.LocationId);
            eventModel.Location = existingLocation;

            var djs = await _ctx.Djs.Where(d => djIds.Contains(d.DjId)).ToListAsync();
            eventModel.Djs = djs;
            if (eventModel.DateFrom > eventModel.DateTo)
            {
                throw new Exception("La data di inizio non può avvenire dopo la data di fine.");
            }

            // Assuming ticketTypesId and ticketQuantities are parallel lists with matching indices
            for (int i = 0; i < ticketTypesId.Count; i++)
            {
                var ticketTypeId = ticketTypesId[i];
                var ticketQuantity = ticketQuantities[i];

                var ticketType = await _ctx.TicketTypes.FindAsync(ticketTypeId);
                if (ticketType != null)
                {
                    var eventTicketType = new EventTicketType
                    {
                        Event = eventModel,
                        TicketType = ticketType,
                        AvailableQuantity = ticketQuantity // This value comes from the form
                    };
                    _ctx.EventTicketType.Add(eventTicketType);
                }
            }

            var eventImages = ImageHelper.HandleEventImg(imageStreams, eventModel);
            eventModel.EventImgs.AddRange(eventImages);

            _ctx.Events.Add(eventModel);
            await _ctx.SaveChangesAsync();

            return eventModel;
        }


        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            var evento = await _ctx.Events
                .Include(e => e.Location)  // Include la Location
                .Include(e => e.Djs)       // Include i DJ
                .Include(e => e.EventImgs) // Include le immagini
                .Include(e => e.Comments) // Include i commenti
                .ThenInclude(c => c.User)
                .Include(e => e.TicketTypes)
                .Include(e => e.EventTicketType)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
            return evento!;
        }



        public async Task<Event> UpdateEventAsync(Event eventModel, List<int> djIds, List<MemoryStream> replaceImageStreams, List<MemoryStream> additionalImageStreams, List<int> ticketTypesId)
        {
            var existingEvent = await _ctx.Events
                .Include(e => e.Djs)
                .Include(e => e.EventImgs)
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.EventId == eventModel.EventId);

            var existingLocation = await _ctx.Locations.FindAsync(eventModel.LocationId);
            existingEvent!.LocationId = eventModel.LocationId;
            existingEvent.Location = existingLocation!;

            var djs = await _ctx.Djs.Where(d => djIds.Contains(d.DjId)).ToListAsync();
            existingEvent.Djs.Clear();
            existingEvent.Djs.AddRange(djs);

            var ticketTypes = await _ctx.TicketTypes.Where(t => ticketTypesId.Contains(t.TicketTypeId)).ToListAsync();
            existingEvent.TicketTypes.Clear();
            existingEvent.TicketTypes.AddRange(ticketTypes);

            existingEvent.Name = eventModel.Name;
            existingEvent.Description = eventModel.Description;
            existingEvent.DateFrom = eventModel.DateFrom;
            existingEvent.DateTo = eventModel.DateTo;
            existingEvent.HostName = eventModel.HostName;

            if (replaceImageStreams != null && replaceImageStreams.Any())
            {
                existingEvent.EventImgs.Clear();
                var replacementImages = ImageHelper.HandleEventImg(replaceImageStreams, existingEvent);
                existingEvent.EventImgs.AddRange(replacementImages);
            }

            if (additionalImageStreams != null && additionalImageStreams.Any())
            {
                var additionalImages = ImageHelper.HandleEventImg(additionalImageStreams, existingEvent);
                existingEvent.EventImgs.AddRange(additionalImages);
            }

            await _ctx.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventModel = await _ctx.Events
                .Include(e => e.EventImgs)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            _ctx.EventImgs.RemoveRange(eventModel!.EventImgs);

            _ctx.Events.Remove(eventModel);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
