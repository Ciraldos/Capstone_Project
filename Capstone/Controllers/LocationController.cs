using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: Location
        public async Task<ActionResult> List()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return View(locations);
        }

        // GET: Location/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);


            return View(location);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Location location)
        {
            if (ModelState.IsValid)
            {
                await _locationService.CreateLocationAsync(location);
                return RedirectToAction("List");
            }
            return View(location);
        }

        // GET: Location/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);


            return View(location);
        }

        // POST: Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                var updatedLocation = await _locationService.UpdateLocationAsync(location);


                return RedirectToAction("List");
            }
            return View(location);
        }

        // GET: Location/Delete/5
        public async Task<ActionResult> Delete(int id, bool confirm = false)
        {
            if (confirm)
            {
                await _locationService.DeleteLocationAsync(id);
                return RedirectToAction("List");
            }

            var locationToDelete = await _locationService.GetLocationByIdAsync(id);

            return View(locationToDelete);
        }
    }
}
