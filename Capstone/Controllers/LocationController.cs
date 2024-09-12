using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationService _locationSvc;
        private readonly IConfiguration _configuration;

        public LocationController(ILocationService locationService, IConfiguration configuration)
        {
            _locationSvc = locationService;
            _configuration = configuration;
        }

        // GET: Location
        public async Task<ActionResult> List()
        {
            var locations = await _locationSvc.GetAllLocationsAsync();
            return View(locations);
        }

        // GET: Location/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var location = await _locationSvc.GetLocationByIdAsync(id);


            return View(location);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;
            return View();
        }

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Location location)
        {
            if (ModelState.IsValid)
            {
                await _locationSvc.CreateLocationAsync(location);
                return RedirectToAction("List");
            }
            return View(location);
        }

        // GET: Location/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var location = await _locationSvc.GetLocationByIdAsync(id);
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;
            return View(location);
        }

        // POST: Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                var updatedLocation = await _locationSvc.UpdateLocationAsync(location);


                return RedirectToAction("List");
            }
            return View(location);
        }

        // GET: Location/Delete/5
        public async Task<ActionResult> Delete(int id, bool confirm = false)
        {
            if (confirm)
            {
                await _locationSvc.DeleteLocationAsync(id);
                return RedirectToAction("List");
            }

            var locationToDelete = await _locationSvc.GetLocationByIdAsync(id);

            return View(locationToDelete);
        }
    }
}
