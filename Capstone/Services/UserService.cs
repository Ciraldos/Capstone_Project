using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Capstone.Services.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _ctx;
        private readonly IPasswordHelper _passwordHelper;
        public UserService(DataContext dataContext, IPasswordHelper passwordHelper)
        {
            _ctx = dataContext;
            _passwordHelper = passwordHelper;

        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _ctx.Users.Include(u => u.Roles).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _ctx.Users.Include(u => u.Roles).Include(u => u.Genres).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            _ctx.Users.Remove(user!);
            await _ctx.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)  // New method
        {
            var user = await _ctx.Users.FindAsync(userId);

            if (!_passwordHelper.VerifyPassword(oldPassword, user!.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = _passwordHelper.HashPassword(newPassword);
            _ctx.Update(user);
            await _ctx.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
        {
            var user = await _ctx.Users.FindAsync(userId);

            var emailExists = await _ctx.Users.AnyAsync(u => u.Email == newEmail && u.UserId != userId);
            if (emailExists)
            {
                return false;
            }

            user!.Email = newEmail;
            _ctx.Update(user);
            await _ctx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserProfileImageAsync(int userId, MemoryStream imageStream)
        {
            var user = await _ctx.Users.FindAsync(userId);

            if (imageStream != null)
            {
                imageStream.Position = 0;
                var imgData = imageStream.ToArray();

                user!.Img = imgData;
                _ctx.Update(user);
                await _ctx.SaveChangesAsync();
            }

            return true;
        }
        public async Task UpdateUserGenresAsync(int userId, List<int> genreIds)
        {
            var user = await _ctx.Users.Include(u => u.Genres).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return;
            }

            // Fetch selected genres from the database
            var genres = await _ctx.Genres.Where(g => genreIds.Contains(g.GenreId)).ToListAsync();

            // Update the user's genres
            user.Genres.Clear();
            user.Genres.AddRange(genres);

            await _ctx.SaveChangesAsync();
        }
    }
}
