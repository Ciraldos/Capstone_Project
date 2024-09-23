using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Capstone.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartSvc;
        private readonly IUserService _userSvc;

        public CartController(ICartService cartService, IUserService userService)
        {
            _cartSvc = cartService;
            _userSvc = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int eventId, int ticketTypeId, int quantity)
        {
            var userId = _userSvc.GetUserId(); // Ottieni l'ID dell'utente loggato
            await _cartSvc.AddToCartAsync(userId, eventId, ticketTypeId, quantity);
            return RedirectToAction("List", "Event");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int eventId, int ticketTypeId)
        {
            var userId = _userSvc.GetUserId();
            var result = await _cartSvc.RemoveFromCartAsync(userId, eventId, ticketTypeId);

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Errore nel rimuovere elemento dal carrello" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int eventId, int ticketTypeId)
        {
            var userId = _userSvc.GetUserId();
            var result = await _cartSvc.UpdateCartAsync(userId, eventId, ticketTypeId);

            if (result)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Non ci sono biglietti disponibili" });
        }



        public async Task<IActionResult> Cart()
        {
            var userId = _userSvc.GetUserId();  // Assumendo che GetUserId() recuperi l'ID dell'utente corrente

            // Recupera tutti i cart items associati all'utente corrente
            var cartItems = await _cartSvc.GetCartItemsByUserIdAsync(userId);

            if (!cartItems.Any())
            {
                return View(new List<CartItem>());  // Se il carrello è vuoto, restituisce una vista vuota
            }

            // Recupera il carrello dell'utente (se necessario per ulteriori dettagli)
            await _cartSvc.GetCartByUserIdAsync(userId);

            return View(cartItems);
        }



        [HttpPost]
        public async Task<IActionResult> Purchase()
        {
            var userId = _userSvc.GetUserId();

            var cartItems = await _cartSvc.GetCartItemsByUserIdAsync(userId);

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
                var userId = _userSvc.GetUserId();
                await _cartSvc.PurchaseCartAsync(userId);
                return View("ConfirmPurchase"); // Optionally show a confirmation page
            }
            else
            {
                return View("PaymentFailed");
            }
        }

    }
}
