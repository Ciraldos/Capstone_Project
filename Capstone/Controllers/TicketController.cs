using Capstone.Context;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Controllers
{
    public class TicketController : Controller
    {
        private readonly DataContext _ctx;
        private readonly IUserService _userSvc;
        public TicketController(DataContext dataContext, IUserService userService)
        {
            _ctx = dataContext;
            _userSvc = userService;
        }
        public async Task<IActionResult> List()
        {
            var userId = _userSvc.GetUserId(); // Ottieni l'ID dell'utente loggato
            var tickets = await _ctx.Tickets
                .Include(t => t.Event)       // Assicurati che l'evento sia caricato
                .ThenInclude(t => t.EventImgs)
                .Include(t => t.Event)       // Assicurati che l'evento sia caricato
                .ThenInclude(t => t.Location)
                .Include(t => t.TicketType)  // Assicurati che il tipo di biglietto sia caricato
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return View(tickets);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ctx.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            _ctx.Tickets.Remove(ticket);
            await _ctx.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
