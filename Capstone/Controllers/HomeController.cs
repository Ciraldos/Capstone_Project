using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Capstone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventSvc;
        private readonly IDjService _djSvc;

        public HomeController(ILogger<HomeController> logger, IEventService eventSvc, IDjService djSvc)
        {
            _eventSvc = eventSvc;
            _logger = logger;
            _djSvc = djSvc;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventSvc.GetPopularEventsAsync();
            return View(events);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Il nome non può essere vuoto.");
            }

            var events = await _eventSvc.SearchEventsAsync(name);
            var djs = await _djSvc.SearchDjsAsync(name);

            var result = new
            {
                Events = events,
                DJs = djs
            };

            return Ok(result);

        }
    }
}
