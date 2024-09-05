using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly DataContext _dataContext;


        public CartController(ICartService cartService, DataContext dataContext)
        {
            _cartService = cartService;
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int eventId, int ticketTypeId, int quantity)
        {
            var userId = GetUserId(); // Ottieni l'ID dell'utente loggato
            await _cartService.AddToCartAsync(userId, eventId, ticketTypeId, quantity);
            return RedirectToAction("List", "Event");
        }

        public async Task<IActionResult> Cart()
        {
            var userId = GetUserId();  // Assumendo che GetUserId() recuperi l'ID dell'utente corrente

            // Recupera tutti i cart items associati all'utente corrente
            var cartItems = await _dataContext.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Include(ci => ci.Event)
                .Include(ci => ci.TicketType)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return View(new List<CartItem>());  // Se il carrello è vuoto, restituisce una vista vuota
            }

            // Recupera il carrello dell'utente (se necessario per ulteriori dettagli)
            var cart = await _dataContext.Carts
                .Where(c => c.UserId == userId)
                .SingleOrDefaultAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase()
        {
            var userId = GetUserId(); // Ottieni l'ID dell'utente loggato
            await _cartService.PurchaseCartAsync(userId);
            return RedirectToAction("Cart"); // Vai alla pagina dei ticket dopo l'acquisto
        }

        private int GetUserId()
        {
            // Implementa un metodo per ottenere l'ID dell'utente loggato, ad esempio:
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
