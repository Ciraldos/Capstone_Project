using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventSvc;
        private readonly ILocationService _locationSvc;
        private readonly IDjService _djSvc;
        private readonly IReviewService _reviewSvc;
        private readonly ITicketTypeService _ticketTypeService;
        private readonly DataContext _dataContext;


        public EventController(IEventService eventService, ILocationService locationService, IDjService djService, IReviewService reviewService, DataContext dataContext, ITicketTypeService ticketTypeService)
        {
            _eventSvc = eventService;
            _locationSvc = locationService;
            _djSvc = djService;
            _reviewSvc = reviewService;
            _dataContext = dataContext;
            _ticketTypeService = ticketTypeService;
        }

        // GET: Event
        public async Task<ActionResult> List()
        {
            var events = await _eventSvc.GetAllEventsAsync();
            return View(events);
        }

        // GET: Event/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var eventDetails = await _eventSvc.GetEventByIdAsync(id);
            var reviews = await _reviewSvc.GetReviewsByEventIdAsync(id);
            ViewBag.Reviews = reviews;

            return View(eventDetails);
        }

        // GET: Event/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Event eventModel, List<int> djIds, List<IFormFile> imageFiles, List<int> ticketTypesIds, List<int> ticketQuantities)
        {
            var imageStreams = new List<MemoryStream>();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageStream = new MemoryStream();
                    await imageFile.CopyToAsync(imageStream);
                    imageStreams.Add(imageStream);
                }
            }
            try
            {
                await _eventSvc.CreateEventAsync(eventModel, djIds, imageStreams, ticketTypesIds, ticketQuantities);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
            return View(eventModel);
        }

        // GET: Event/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var eventToEdit = await _eventSvc.GetEventByIdAsync(id);

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            ViewBag.TicketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
            return View(eventToEdit);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Event eventModel, List<int> djIds, List<IFormFile> imageFiles, List<IFormFile> additionalImageFiles, List<int> ticketTypesIds)
        {
            var replaceImageStreams = new List<MemoryStream>();
            var additionalImageStreams = new List<MemoryStream>();

            // Handle image files for replacement
            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageStream = new MemoryStream();
                    await imageFile.CopyToAsync(imageStream);
                    replaceImageStreams.Add(imageStream);
                }
            }

            // Handle additional image files to add to existing ones
            foreach (var additionalImageFile in additionalImageFiles)
            {
                if (additionalImageFile != null && additionalImageFile.Length > 0)
                {
                    var additionalImageStream = new MemoryStream();
                    await additionalImageFile.CopyToAsync(additionalImageStream);
                    additionalImageStreams.Add(additionalImageStream);
                }
            }

            try
            {
                await _eventSvc.UpdateEventAsync(eventModel, djIds, replaceImageStreams, additionalImageStreams, ticketTypesIds);
                return RedirectToAction("List");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            return View(eventModel);
        }


        // GET: Event/Delete/5
        public async Task<ActionResult> Delete(int id, bool confirm = false)
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
            // Get the currently logged-in user's ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, error = "Unable to retrieve User ID." });
            }

            // Check if the user has already posted a review for this event
            var existingReview = await _dataContext.Reviews
                .FirstOrDefaultAsync(r => r.EventId == reviewModel.EventId && r.UserId == userId);

            if (existingReview != null)
            {
                // User has already posted a review, return an error
                ModelState.AddModelError("", "You have already posted a review for this event.");
                return RedirectToAction("Details", new { id = reviewModel.EventId });
            }

            // Proceed with creating the new review
            reviewModel.UserId = userId;
            var imageStreams = new List<MemoryStream>();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageStream = new MemoryStream();
                    await imageFile.CopyToAsync(imageStream);
                    imageStreams.Add(imageStream);
                }
            }

            await _reviewSvc.CreateReviewAsync(reviewModel, imageStreams);

            return RedirectToAction("Details", new { id = reviewModel.EventId });
        }



        [HttpPost]
        public async Task<IActionResult> EditReview(Review reviewModel, List<IFormFile> imageFiles)
        {
            var imageStreams = new List<MemoryStream>();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageStream = new MemoryStream();
                    await imageFile.CopyToAsync(imageStream);
                    imageStreams.Add(imageStream);
                }
            }

            // Retrieve the logged-in user's UserId from the claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                reviewModel.UserId = userId; // Assign the UserId to the review
            }
            else
            {
                return Json(new { success = false, error = "Unable to retrieve User ID." });
            }

            var updatedReview = await _reviewSvc.UpdateReviewAsync(reviewModel, imageStreams, null);

            // Return success response as JSON
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

        // POST: Review/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteReview(int reviewId, int eventId)
        {
            await _reviewSvc.DeleteReviewAsync(reviewId);
            return RedirectToAction("Details", new { id = eventId });
        }
    }
}
