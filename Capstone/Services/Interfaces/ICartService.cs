using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> AddToCartAsync(int userId, int eventId, int ticketTypeId, int quantity);
        Task PurchaseCartAsync(int userId);
        Task<string> GenerateUniqueTicketNumberAsync();
    }
}
