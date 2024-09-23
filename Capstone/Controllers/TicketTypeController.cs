using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class TicketTypeController : Controller
    {
        private readonly ITicketTypeService _ticketTypeSvc;

        public TicketTypeController(ITicketTypeService ticketTypeService)
        {
            _ticketTypeSvc = ticketTypeService;
        }

        [Authorize(Policy = "AdminOrMasterPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrMasterPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                await _ticketTypeSvc.CreateAsync(ticketType);
                return RedirectToAction("List");
            }
            return View(ticketType);
        }

        [Authorize(Policy = "AdminOrMasterPolicy")]
        public async Task<IActionResult> Edit(int id)
        {
            var ticketType = await _ticketTypeSvc.GetTicketTypeByIdAsync(id);
            return View(ticketType);
        }


        [HttpPost]
        [Authorize(Policy = "AdminOrMasterPolicy")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                await _ticketTypeSvc.UpdateAsync(ticketType);
                return RedirectToAction("List");
            }
            return View(ticketType);
        }

        [Authorize(Policy = "AdminOrMasterPolicy")]
        public async Task<IActionResult> List()
        {
            var ticketTypes = await _ticketTypeSvc.GetAllTicketTypesAsync();
            return View(ticketTypes);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrMasterPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketTypeSvc.DeleteAsync(id);
            return RedirectToAction("List");
        }

        [Authorize(Policy = "AdminOrMasterPolicy")]
        public async Task<IActionResult> Details(int id)
        {
            var ticketType = await _ticketTypeSvc.GetTicketTypeByIdAsync(id);
            return View(ticketType);
        }
    }
}
