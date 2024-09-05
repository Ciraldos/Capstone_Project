using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventSvc;
        private readonly ILocationService _locationSvc;
        private readonly IDjService _djSvc;


        public EventController(IEventService eventService, ILocationService locationService, IDjService djService)
        {
            _eventSvc = eventService;
            _locationSvc = locationService;
            _djSvc = djService;
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

            return View(eventDetails);
        }

        // GET: Event/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Event eventModel, List<int> djIds, List<IFormFile> imageFiles)
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
                await _eventSvc.CreateEventAsync(eventModel, djIds, imageStreams);
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

        // GET: Event/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var eventToEdit = await _eventSvc.GetEventByIdAsync(id);

            ViewBag.Locations = await _locationSvc.GetAllLocationsAsync();
            ViewBag.DJs = await _djSvc.GetAllDjAsync();
            return View(eventToEdit);
        }

        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Event eventModel, List<int> djIds, List<IFormFile> imageFiles, List<IFormFile> additionalImageFiles)
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
                await _eventSvc.UpdateEventAsync(eventModel, djIds, replaceImageStreams, additionalImageStreams);
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

    }
}
