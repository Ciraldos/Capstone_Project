using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> AddToCartAsync(int userId, int eventId, int ticketTypeId, int quantity);
        Task<bool> RemoveFromCartAsync(int userId, int eventId, int ticketTypeId);
        Task<bool> UpdateCartAsync(int userId, int eventId, int ticketTypeId);
        Task PurchaseCartAsync(int userId);
        Task<string> GenerateUniqueTicketNumberAsync();
        Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId);
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<int> GetCartItemsCount(int userId);

    }
}
