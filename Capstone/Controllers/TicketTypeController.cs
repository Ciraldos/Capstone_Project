using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class TicketTypeController : Controller
    {
        private readonly ITicketTypeService _ticketTypeService;

        public TicketTypeController(ITicketTypeService ticketTypeService)
        {
            _ticketTypeService = ticketTypeService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                await _ticketTypeService.CreateAsync(ticketType);
                return RedirectToAction("List");
            }
            return View(ticketType);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
            return View(ticketType);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                await _ticketTypeService.UpdateAsync(ticketType);
                return RedirectToAction("List");
            }
            return View(ticketType);
        }

        public async Task<IActionResult> List()
        {
            var ticketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
            return View(ticketTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketTypeService.DeleteAsync(id);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
            return View(ticketType);
        }
    }
}
