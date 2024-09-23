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
                .ThenInclude(ci => ci.EventImgs)
                .Include(ci => ci.Event)
                .ThenInclude(ci => ci.Location)
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
            var cart = await _ctx.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { User = user };
                await _ctx.Carts.AddAsync(cart);
                await _ctx.SaveChangesAsync();
            }

            var eventTicketType = await _ctx.EventTicketType
                .FirstOrDefaultAsync(et => et.EventId == eventId && et.TicketTypeId == ticketTypeId);

            if (eventTicketType == null)
            {
                throw new InvalidOperationException("Il tipo di biglietto selezionato non esiste.");
            }

            // Controlla se la quantità richiesta è maggiore della disponibilità corrente
            int availableToAdd = eventTicketType.AvailableQuantity;

            if (availableToAdd <= 0)
            {
                throw new InvalidOperationException("Non ci sono più biglietti disponibili.");
            }

            // Limita la quantità da aggiungere solo ai biglietti disponibili
            int ticketsToAdd = Math.Min(quantity, availableToAdd);

            var cartItem = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.EventId == eventId && ci.TicketTypeId == ticketTypeId);

            if (cartItem != null)
            {
                // Aggiungi solo i biglietti disponibili al carrello
                cartItem.Quantity += ticketsToAdd;
            }
            else
            {
                cartItem = new CartItem
                {
                    Cart = cart,
                    Quantity = ticketsToAdd, // Aggiungi solo quelli disponibili
                    Event = await _ctx.Events.FindAsync(eventId),
                    TicketType = await _ctx.TicketTypes.FindAsync(ticketTypeId)
                };
                _ctx.CartItems.Add(cartItem);
            }

            // Aggiorna la quantità disponibile nel database
            eventTicketType.AvailableQuantity -= ticketsToAdd;

            await _ctx.SaveChangesAsync();

            // Ritorna il carrello aggiornato
            return cart;
        }


        public async Task<bool> RemoveFromCartAsync(int userId, int eventId, int ticketTypeId)
        {
            var cart = await _ctx.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return false;

            var cartItem = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.EventId == eventId && ci.TicketTypeId == ticketTypeId);

            if (cartItem == null) return false;

            var eventTicketType = await _ctx.EventTicketType
                .FirstOrDefaultAsync(et => et.EventId == eventId && et.TicketTypeId == ticketTypeId);

            if (eventTicketType != null)
            {
                eventTicketType.AvailableQuantity++; // Incrementa la quantità disponibile di biglietti
            }

            // Decrementa la quantità nel carrello
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--; // Solo decremento se c'è più di 1 biglietto
            }
            else
            {
                _ctx.CartItems.Remove(cartItem); // Rimuove l'articolo solo se la quantità diventa zero
            }

            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartAsync(int userId, int eventId, int ticketTypeId)
        {
            var cart = await _ctx.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return false;

            var cartItem = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.EventId == eventId && ci.TicketTypeId == ticketTypeId);

            if (cartItem == null) return false; // Assumi che l'elemento esista già, ma in caso contrario, esci

            var eventTicketType = await _ctx.EventTicketType
                .FirstOrDefaultAsync(et => et.EventId == eventId && et.TicketTypeId == ticketTypeId);

            if (eventTicketType == null) return false; // Tipo di biglietto non trovato

            // Totale biglietti disponibili per l'evento (inclusi quelli già nel carrello)
            var totalAvailableIncludingCart = eventTicketType.AvailableQuantity + cartItem.Quantity;

            // Verifica che l'utente non abbia già raggiunto il limite di biglietti disponibili
            if (cartItem.Quantity < totalAvailableIncludingCart)
            {
                // Incrementa la quantità nel carrello
                cartItem.Quantity++;

                // Decrementa la disponibilità solo se l'operazione è andata a buon fine
                eventTicketType.AvailableQuantity--;
            }
            else
            {
                return false; // Non ci sono abbastanza biglietti disponibili
            }

            await _ctx.SaveChangesAsync();
            return true; // Quantità aggiornata con successo
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
            string emailBody = @"
                <!DOCTYPE html>
                <html lang='it'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 20px;
                        }
                        .container {
                            max-width: 600px;
                            margin: auto;
                            background: white;
                            border-radius: 8px;
                            padding: 20px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }
                        h1 {
                            color: #333;
                        }
                        ul {
                            list-style-type: none;
                            padding: 0;
                        }
                        li {
                            background: #e9e9e9;
                            margin: 10px 0;
                            padding: 15px;
                            border-radius: 5px;
                        }
                        .ticket-info {
                            color: #555;
                            border-radius: 15px;
                        }
                        .footer {
                            margin-top: 20px;
                            text-align: center;
                            font-size: 0.9em;
                            color: #888;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Grazie per il tuo acquisto!</h1>
                        <p>I tuoi biglietti sono stati generati con successo. Ecco i dettagli:</p>
                        <ul>";

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
                    emailBody += $@"
                                <li>
                                    <div class='ticket-info'>
                                        Biglietto per: <strong>{cartItem.Event.Name}</strong><br>
                                        Tipo: <strong>{cartItem.TicketType.TicketTypeName}</strong><br>
                                        Descrizione: <strong>{cartItem.TicketType.TicketTypeDescription}</strong><br>
                                        Codice: <strong>{ticketNumber}</strong>
                                    </div>
                                </li>";
                }
            }

            emailBody += @"
                        </ul>
                        <p>Grazie per aver scelto il nostro servizio! I tuoi biglietti saranno disponibili nel tuo profilo.</p>
                        <div class='footer'>
                            <p>Se hai domande, non esitare a contattarci.</p>
                            <p>&copy; 2024 Ton?ght. Tutti i diritti riservati.</p>
                        </div>
                    </div>
                </body>
                </html>";

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
