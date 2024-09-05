using Capstone.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class TicketController : Controller
    {
        private readonly DataContext _dataContext;
        public TicketController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> MyTickets()
        {
            var userId = GetUserId(); // Ottieni l'ID dell'utente loggato
            var tickets = await _dataContext.Tickets
                .Include(t => t.Event)       // Assicurati che l'evento sia caricato
                .Include(t => t.TicketType)  // Assicurati che il tipo di biglietto sia caricato
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return View(tickets);
        }

        private int GetUserId()
        {
            // Implementa un metodo per ottenere l'ID dell'utente loggato, ad esempio:
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
