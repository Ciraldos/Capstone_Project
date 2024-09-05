using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;
        private readonly IQrCodeService _qrCodeService;

        public CartService(DataContext dataContext, IQrCodeService qrCodeService)
        {
            _dataContext = dataContext;
            _qrCodeService = qrCodeService;
        }

        public async Task<Cart> AddToCartAsync(int userId, int eventId, int ticketTypeId, int quantity)
        {

            // Cerco il carrello dell'utente dentro il database. Se non esiste lo creo.
            var cart = await _dataContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { User = user };
                await _dataContext.Carts.AddAsync(cart);
                await _dataContext.SaveChangesAsync();
            }

            var cartItem = await _dataContext.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.EventId == eventId && ci.TicketTypeId == ticketTypeId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    Cart = cart,
                    Quantity = quantity,
                    Event = await _dataContext.Events.FindAsync(eventId),
                    TicketType = await _dataContext.TicketTypes.FindAsync(ticketTypeId)
                };
                _dataContext.CartItems.Add(cartItem);
            }
            await _dataContext.SaveChangesAsync();
            return cart;
        }

        public async Task PurchaseCartAsync(int userId)
        {
            // Trova il carrello e i relativi articoli
            var cart = await _dataContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                throw new InvalidOperationException("No cart found for this user");

            var cartItems = await _dataContext.CartItems
                .Where(ci => ci.CartId == cart.CartId)
                .Include(ci => ci.Event)
                .Include(ci => ci.TicketType)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("No items in cart");

            foreach (var cartItem in cartItems)
            {
                for (int i = 0; i < cartItem.Quantity; i++)
                {
                    var ticketNumber = await GenerateUniqueTicketNumberAsync();
                    var ticket = new Ticket
                    {
                        PurchaseDate = DateTime.Now,
                        NumTicket = ticketNumber,
                        TicketType = cartItem.TicketType,
                        User = await _dataContext.Users.FindAsync(userId),
                        Event = cartItem.Event,
                        QRCodeImage = _qrCodeService.GenerateQRCode(ticketNumber)
                    };
                    _dataContext.Tickets.Add(ticket);
                }
            }

            // Rimuove gli articoli dal carrello dopo l'acquisto
            _dataContext.CartItems.RemoveRange(cartItems);
            await _dataContext.SaveChangesAsync();
        }


        public async Task<string> GenerateUniqueTicketNumberAsync()
        {
            string ticketNumber;
            bool isUnique;
            do
            {
                // Genera un numero di biglietto
                ticketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                // Verifica se il numero di biglietto esiste già
                isUnique = !await _dataContext.Tickets.AnyAsync(t => t.NumTicket == ticketNumber);
            } while (!isUnique); // Continua a generare fino a trovare un numero unico

            return ticketNumber;
        }

    }
}
