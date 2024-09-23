using Capstone.Context;
using Capstone.Models;
using Capstone.Models.ViewModels;
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
                .Include(e => e.Genres)
                .Include(e => e.TicketTypes)
                .ToListAsync();
        }

        // Funzione per ottenere gli eventi più popolari basati sui commenti degli ultimi 3 giorni
        public async Task<List<Event>> GetPopularEventsAsync()
        {
            var threeDaysAgo = DateTime.Now.AddDays(-3);

            return await _ctx.Events
                .Include(e => e.Location)
                .Include(e => e.Djs)
                .Include(e => e.EventImgs)
                .Include(e => e.Genres)
                .Include(e => e.Comments)
                .Include(e => e.TicketTypes)
                .Where(e => e.Comments.Count(c => c.PublishedAt >= threeDaysAgo) >= 3) // Controlla i commenti degli ultimi 3 giorni
                .OrderByDescending(e => e.DateFrom)
                .Take(3)
                .ToListAsync();
        }


        public async Task<Event> CreateEventAsync(Event eventModel, List<int> djIds, List<int> selectedGenres, List<IFormFile> imageFiles, List<int> ticketTypesId, List<int> ticketQuantities)
        {
            var existingLocation = await _ctx.Locations.FindAsync(eventModel.LocationId);
            eventModel.Location = existingLocation;

            var djs = await _ctx.Djs.Where(d => djIds.Contains(d.DjId)).ToListAsync();
            eventModel.Djs = djs;

            var genres = await _ctx.Genres.Where(g => selectedGenres.Contains(g.GenreId)).ToListAsync();
            eventModel.Genres = genres;

            if (eventModel.DateFrom > eventModel.DateTo)
            {
                throw new Exception("La data di inizio non può avvenire dopo la data di fine.");
            }

            // Handling ticket types and quantities
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
                        AvailableQuantity = ticketQuantity
                    };
                    _ctx.EventTicketType.Add(eventTicketType);
                }
            }
            _ctx.Events.Add(eventModel);
            await _ctx.SaveChangesAsync();

            // Create folder for event images
            var eventFolder = Path.Combine("wwwroot", "uploads", "events", eventModel.EventId.ToString());
            if (!Directory.Exists(eventFolder))
            {
                Directory.CreateDirectory(eventFolder);
            }

            // Handle images saving to file system
            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(eventFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Save image info in the database
                    var eventImg = new EventImg
                    {
                        Event = eventModel,
                        FilePath = $"/uploads/events/{eventModel.EventId}/{fileName}"
                    };
                    eventModel.EventImgs.Add(eventImg);
                }
            }

            _ctx.Events.Update(eventModel);
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
                .Include(e => e.Genres)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
            return evento!;
        }



        public async Task<Event> UpdateEventAsync(Event eventModel, List<int> djIds, List<int> selectedGenres, List<string> replaceImagePaths, List<string> additionalImagePaths, List<int> ticketTypesId, List<int> ticketQuantities)
        {
            var existingEvent = await _ctx.Events
                .Include(e => e.Djs)
                .Include(e => e.Location)
                .Include(e => e.EventImgs)
                .Include(e => e.TicketTypes)
                .Include(e => e.Genres)
                .Include(e => e.EventTicketType)
                .FirstOrDefaultAsync(e => e.EventId == eventModel.EventId);

            var existingLocation = await _ctx.Locations.FindAsync(eventModel.LocationId);
            existingEvent!.LocationId = eventModel.LocationId;
            existingEvent.Location = existingLocation!;

            // Aggiornamento dei DJ
            var djs = await _ctx.Djs.Where(d => djIds.Contains(d.DjId)).ToListAsync();
            existingEvent.Djs.Clear();
            existingEvent.Djs.AddRange(djs);

            // Aggiornamento dei generi musicali
            var genres = await _ctx.Genres.Where(g => selectedGenres.Contains(g.GenreId)).ToListAsync();
            existingEvent.Genres.Clear();
            existingEvent.Genres.AddRange(genres);

            // Aggiornamento dei tipi di biglietti
            existingEvent.EventTicketType.Clear();  // Svuota la tabella ponte EventTicketType
            for (int i = 0; i < ticketTypesId.Count; i++)
            {
                var ticketTypeId = ticketTypesId[i];
                var quantity = ticketQuantities[i];

                // Aggiungi una nuova riga nella tabella ponte per ogni tipo di biglietto
                existingEvent.EventTicketType.Add(new EventTicketType
                {
                    EventId = existingEvent.EventId,
                    TicketTypeId = ticketTypeId,
                    AvailableQuantity = quantity
                });
            }

            // Aggiornamento delle informazioni dell'evento
            existingEvent.Name = eventModel.Name;
            existingEvent.Description = eventModel.Description;
            existingEvent.DateFrom = eventModel.DateFrom;
            existingEvent.DateTo = eventModel.DateTo;
            existingEvent.HostName = eventModel.HostName;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events", existingEvent.EventId.ToString());

            // Sostituzione delle immagini esistenti
            if (replaceImagePaths != null && replaceImagePaths.Any())
            {
                // Trova le immagini obsolete
                var existingPaths = existingEvent.EventImgs.Select(ei => ei.FilePath).ToList();
                var pathsToDelete = existingPaths.Except(replaceImagePaths).ToList();

                // Elimina le immagini obsolete dal filesystem
                foreach (var path in pathsToDelete)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // Aggiorna il database con le nuove immagini
                existingEvent.EventImgs.Clear();
                foreach (var imgPath in replaceImagePaths)
                {
                    existingEvent.EventImgs.Add(new EventImg { Event = existingEvent, FilePath = imgPath });
                }
            }

            // Aggiunta delle nuove immagini
            if (additionalImagePaths != null && additionalImagePaths.Any())
            {
                foreach (var additionalImgPath in additionalImagePaths)
                {
                    existingEvent.EventImgs.Add(new EventImg { Event = existingEvent, FilePath = additionalImgPath });
                }
            }

            await _ctx.SaveChangesAsync();
            return existingEvent;
        }


        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventModel = await _ctx.Events
                .Include(e => e.EventImgs)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventModel == null) return false;

            // Directory path where event images are stored (assuming the path structure)
            var eventDirectory = Path.Combine("wwwroot", "uploads", "events", eventId.ToString());

            // Delete images from file system
            foreach (var img in eventModel.EventImgs)
            {
                var imagePath = Path.Combine(eventDirectory, img.FilePath);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            // Delete the directory after images are removed
            if (Directory.Exists(eventDirectory))
            {
                Directory.Delete(eventDirectory, true); // 'true' to delete recursively
            }

            // Remove the images and event from the database
            _ctx.EventImgs.RemoveRange(eventModel.EventImgs);
            _ctx.Events.Remove(eventModel);

            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<List<EventViewModel>> SearchEventsAsync(string name)
        {
            try
            {
                var events = await _ctx.Events
                    .Where(e => e.Name.Contains(name) || e.Description.Contains(name))
                    .Select(e => new EventViewModel
                    {
                        EventId = e.EventId,
                        Name = e.Name,
                        Description = e.Description,
                        DateFrom = e.DateFrom,
                        DateTo = e.DateTo,
                        Location = e.Location.LocationName,
                        ImageUrl = e.EventImgs.FirstOrDefault().FilePath // Se ci sono immagini
                    })
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                // Log dell'eccezione
                Console.WriteLine(ex.ToString());
                throw; // Rilancia l'eccezione per gestirla nel controller
            }
        }
    }
}
