using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext _ctx;
        private readonly IEmailService _emailService;
        private readonly IQrCodeService _qrCodeService;

        public CartService(DataContext dataContext, IQrCodeService qrCodeService, IEmailService emailService)
        {
            _ctx = dataContext;
            _qrCodeService = qrCodeService;
            _emailService = emailService;
        }
        public async Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId)
        {
            return await _ctx.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Include(ci => ci.Event)
                .Include(ci => ci.TicketType)
                .ToListAsync();
        }
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cart = await _ctx.Carts
                .Where(c => c.UserId == userId)
                .SingleOrDefaultAsync();
            return cart!;
        }
        public async Task<Cart> AddToCartAsync(int userId, int eventId, int ticketTypeId, int quantity)
        {
            // Trova il carrello dell'utente dentro il database. Se non esiste lo creo.
            var cart = await _ctx.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { User = user };
                await _ctx.Carts.AddAsync(cart);
                await _ctx.SaveChangesAsync();
            }

            // Controlla la disponibilità dei biglietti
            var eventTicketType = await _ctx.EventTicketType
                .FirstOrDefaultAsync(et => et.EventId == eventId && et.TicketTypeId == ticketTypeId);

            if (eventTicketType == null || eventTicketType.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Non ci sono abbastanza biglietti disponibili per il tipo di biglietto selezionato.");
            }

            // Se il biglietto esiste già nel carrello, aggiorna la quantità
            var cartItem = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.EventId == eventId && ci.TicketTypeId == ticketTypeId);

            if (cartItem != null)
            {
                if (eventTicketType.AvailableQuantity < cartItem.Quantity + quantity)
                {
                    throw new InvalidOperationException("Non ci sono abbastanza biglietti disponibili per il tipo di biglietto selezionato.");
                }
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    Cart = cart,
                    Quantity = quantity,
                    Event = await _ctx.Events.FindAsync(eventId),
                    TicketType = await _ctx.TicketTypes.FindAsync(ticketTypeId)
                };
                _ctx.CartItems.Add(cartItem);
            }

            // Riduci la quantità disponibile nel database
            eventTicketType.AvailableQuantity -= quantity;

            await _ctx.SaveChangesAsync();
            return cart;
        }


        public async Task PurchaseCartAsync(int userId)
        {
            // Trova il carrello e i relativi articoli
            var cart = await _ctx.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                throw new InvalidOperationException("Nessun carrello trovato per questo utente");

            var cartItems = await _ctx.CartItems
                .Where(ci => ci.CartId == cart.CartId)
                .Include(ci => ci.Event)
                .Include(ci => ci.TicketType)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("Nessun articolo nel carrello");

            var user = await _ctx.Users.FindAsync(userId);
            string emailBody = "<h1>Grazie per il tuo acquisto!</h1><ul>";

            var generatedTickets = new List<Ticket>();

            foreach (var cartItem in cartItems)
            {
                for (int i = 0; i < cartItem.Quantity; i++)
                {
                    var ticketNumber = await GenerateUniqueTicketNumberAsync();
                    var qrCodeImage = _qrCodeService.GenerateQRCode(ticketNumber);

                    var ticket = new Ticket
                    {
                        PurchaseDate = DateTime.Now,
                        NumTicket = ticketNumber,
                        TicketType = cartItem.TicketType,
                        User = user,
                        Event = cartItem.Event,
                        QRCodeImage = qrCodeImage
                    };
                    _ctx.Tickets.Add(ticket);
                    generatedTickets.Add(ticket);  // Aggiungi il biglietto alla lista dei biglietti generati

                    // Aggiungi dettagli del biglietto all'email
                    emailBody += $"<li>Biglietto per {cartItem.Event.Name}, Tipo: {cartItem.TicketType.TicketTypeName} ,Descrizione:  {cartItem.TicketType.TicketTypeDescription}, Codice: {ticketNumber}</li>";
                }
            }

            emailBody += "</ul>";

            _ctx.CartItems.RemoveRange(cartItems);
            await _ctx.SaveChangesAsync();

            // Invia l'email con i QR code dei biglietti generati
            await _emailService.SendEmailAsync(user.Email, "I tuoi biglietti", emailBody, generatedTickets);
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
                isUnique = !await _ctx.Tickets.AnyAsync(t => t.NumTicket == ticketNumber);
            } while (!isUnique); // Continua a generare fino a trovare un numero unico

            return ticketNumber;
        }

    }
}
