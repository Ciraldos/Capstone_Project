using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> ChangeEmailAsync(int userId, string newEmail);
        Task<bool> UpdateUserProfileImageAsync(int userId, MemoryStream imageStream);
        Task UpdateUserGenresAsync(int userId, List<int> genreIds);
    }
}
