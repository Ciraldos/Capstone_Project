using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;


        public CartController(ICartService cartService, DataContext dataContext, IConfiguration configuration)
        {
            _cartService = cartService;
            _dataContext = dataContext;
            _configuration = configuration;
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
            var userId = GetUserId();

            // Retrieve the cart items and calculate the total price
            var cartItems = await _dataContext.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Include(ci => ci.Event)
                .Include(ci => ci.TicketType)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("No items in cart");

            // Calculate total price (in cents for euros)
            var totalAmount = (long)(cartItems.Sum(ci => ci.Quantity * ci.TicketType.Price) * 100);

            // Create a list of line items for Stripe Checkout
            var lineItems = cartItems.Select(ci => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(ci.TicketType.Price * 100), // Price in cents
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = ci.Event.Name,
                    },
                },
                Quantity = ci.Quantity,
            }).ToList();

            // Create Stripe Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("ConfirmPurchase", "Cart", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("Cart", "Cart", null, Request.Scheme),
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // Redirect to Stripe Checkout
            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmPurchase(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return View("PaymentFailed", "Session ID is missing");
            }

            var service = new SessionService();
            var session = await service.GetAsync(session_id);

            if (session.PaymentStatus == "paid")
            {
                var userId = GetUserId();
                await _cartService.PurchaseCartAsync(userId);
                return View("ConfirmPurchase"); // Optionally show a confirmation page
            }
            else
            {
                return View("PaymentFailed");
            }
        }




        private int GetUserId()
        {
            // Implementa un metodo per ottenere l'ID dell'utente loggato, ad esempio:
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

    }
}
