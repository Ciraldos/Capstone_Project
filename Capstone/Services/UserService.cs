using Capstone.Context;
using Capstone.Helpers;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _ctx;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(DataContext dataContext, IPasswordHelper passwordHelper, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = dataContext;
            _passwordHelper = passwordHelper;
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetUserId()
        {
            // Retrieve the User ID from the claims, assuming it is stored as a string and is convertible to an integer
            var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            else
            {
                throw new InvalidOperationException("User ID is not available or invalid.");
            }
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _ctx.Users.Include(u => u.Roles).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _ctx.Users.Include(u => u.Roles).Include(u => u.Genres).FirstOrDefaultAsync(u => u.UserId == userId);
            return user!;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            _ctx.Users.Remove(user!);
            await _ctx.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _ctx.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!_passwordHelper.VerifyPassword(oldPassword, user.PasswordHash))
            {
                throw new ArgumentException("Incorrect current password.");
            }

            user.PasswordHash = _passwordHelper.HashPassword(newPassword);
            _ctx.Update(user);
            await _ctx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
        {
            if (string.IsNullOrEmpty(newEmail))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            var user = await _ctx.Users.FindAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var emailExists = await _ctx.Users.AnyAsync(u => u.Email == newEmail && u.UserId != userId);
            if (emailExists)
            {
                throw new ArgumentException("The email is already in use by another account.");
            }

            user.Email = newEmail;

            try
            {
                _ctx.Update(user);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or examine the inner exception
                throw new Exception("An error occurred while updating the email: " + ex.InnerException?.Message, ex);
            }

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
