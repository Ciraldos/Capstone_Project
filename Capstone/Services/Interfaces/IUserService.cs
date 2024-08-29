using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
    }
}
