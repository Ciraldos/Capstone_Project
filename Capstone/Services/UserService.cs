using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _ctx;
        public UserService(DataContext dataContext)
        {
            _ctx = dataContext;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _ctx.Users.Include(u => u.Roles).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            _ctx.Users.Remove(user!);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
