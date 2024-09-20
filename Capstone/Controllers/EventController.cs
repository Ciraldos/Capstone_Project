using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventSvc;
        private readonly ILocationService _locationSvc;
        private readonly IDjService _djSvc;
        private readonly IReviewService _reviewSvc;
        private readonly ITicketTypeService _ticketTypeSvc;
        private readonly IGenreService _genreSvc;
        private readonly IConfiguration _configuration;


        public EventController(IEventService eventService, ILocationService locationService, IDjService djService, IReviewService reviewService, ITicketTypeService ticketTypeService, IGenreService genreService, IConfiguration configuration)
        {
            _eventSvc = eventService;
            _locationSvc = locationService;
            _djSvc = djService;
            _reviewSvc = reviewService;
            _ticketTypeSvc = ticketTypeService;
            _genreSvc = genreService;
            _configuration = configuration;
        }

        // GET: Event
        public async Task<IActionResult> List()
        {
            var events = await _eventSvc.GetAllEventsAsync();
            ViewBag.Genres = await _genreSvc.GetAllGenresAsync();
            return View(events);
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await _eventSvc.GetEventByIdAsync(id);
            var reviews = await _reviewSvc.GetReviewsByEventIdAsync(id);
            ViewBag.Reviews = reviews;

            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;

            var giphyApiKey = _configuration["Giphy:ApiKey"];
            ViewBag.GiphyApiKey = giphyApiKey;
            return View(eventDetails);
        }

        // GET: Event/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeSvc.GetAllTicketTypesAsync();
            ViewBag.Genres = await _genreSvc.GetAllGenresAsync();
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventModel, List<int> djIds, List<int> selectedGenres, List<IFormFile> imageFiles, List<int> ticketTypesIds, List<int> ticketQuantities)
        {
            try
            {
                await _eventSvc.CreateEventAsync(eventModel, selectedGenres, djIds, imageFiles, ticketTypesIds, ticketQuantities);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeSvc.GetAllTicketTypesAsync();
            ViewBag.Genres = await _genreSvc.GetAllGenresAsync();
            return View(eventModel);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var eventToEdit = await _eventSvc.GetEventByIdAsync(id);

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeSvc.GetAllTicketTypesAsync();
            ViewBag.Genres = await _genreSvc.GetAllGenresAsync();
            return View(eventToEdit);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event eventModel, List<int> djIds, List<int> selectedGenres, List<IFormFile> imageFiles, List<IFormFile> additionalImageFiles, List<int> ticketTypesIds)
        {
            // Percorsi per i file di immagine
            var replaceImagePaths = new List<string>();
            var additionalImagePaths = new List<string>();

            // Salva le immagini per la sostituzione
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Salva il file nel file system e ottieni il percorso
                        var imagePath = await SaveImageToFileSystem(imageFile, "events", eventModel.EventId);
                        replaceImagePaths.Add(imagePath);
                    }
                }
            }

            // Salva le immagini aggiuntive
            if (additionalImageFiles != null && additionalImageFiles.Any())
            {
                foreach (var additionalImageFile in additionalImageFiles)
                {
                    if (additionalImageFile != null && additionalImageFile.Length > 0)
                    {
                        var additionalImagePath = await SaveImageToFileSystem(additionalImageFile, "events", eventModel.EventId);
                        additionalImagePaths.Add(additionalImagePath);
                    }
                }
            }

            try
            {
                await _eventSvc.UpdateEventAsync(eventModel, djIds, selectedGenres, replaceImagePaths, additionalImagePaths, ticketTypesIds);
                return RedirectToAction("Details", new { id = eventModel.EventId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeSvc.GetAllTicketTypesAsync();
            ViewBag.Genres = await _genreSvc.GetAllGenresAsync();
            return View(eventModel);
        }

        // Metodo di supporto per salvare le immagini nel file system
        private async Task<string> SaveImageToFileSystem(IFormFile imageFile, string folder, int eventId)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder, eventId.ToString());

            // Crea la cartella se non esiste
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Restituisci il percorso relativo da salvare nel database
            return $"/uploads/{folder}/{eventId}/{fileName}";
        }



        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int id, bool confirm = false)
        {
            if (confirm)
            {
                await _eventSvc.DeleteEventAsync(id);
                return RedirectToAction("List");
            }

            var eventToDelete = await _eventSvc.GetEventByIdAsync(id);

            return View(eventToDelete);
        }


        //REVIEWS
        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(Review reviewModel, List<IFormFile> imageFiles)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, error = "Unable to retrieve User ID." });
            }

            var existingReview = await _reviewSvc.GetReviewByEventIdAndUserIdAsync(reviewModel.EventId, userId);

            if (existingReview != null)
            {
                ModelState.AddModelError("", "You have already posted a review for this event.");
                return RedirectToAction("Details", new { id = reviewModel.EventId });
            }

            reviewModel.UserId = userId;
            await _reviewSvc.CreateReviewAsync(reviewModel, imageFiles);

            return RedirectToAction("Details", new { id = reviewModel.EventId });
        }



        [HttpPost]
        public async Task<IActionResult> EditReview(Review reviewModel, List<IFormFile> imageFiles)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, error = "Unable to retrieve User ID." });
            }

            reviewModel.UserId = userId; // Assign the UserId to the review

            var updatedReview = await _reviewSvc.UpdateReviewAsync(reviewModel, imageFiles);

            return Json(new
            {
                success = true,
                review = new
                {
                    title = updatedReview.Title,
                    description = updatedReview.Description,
                    rating = updatedReview.Rating
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                await _reviewSvc.DeleteReviewAsync(reviewId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception details here if needed
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
