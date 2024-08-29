using Capstone.Context;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services.Master
{
    public class MasterService : IMasterService
    {
        private readonly DataContext _ctx;

        public MasterService(DataContext dataContext)
        {
            _ctx = dataContext;
        }

        public async Task<bool> AddRoleToUserAsync(int userId, int roleId)
        {

            var user = await _ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
            var role = await _ctx.Roles.FindAsync(roleId);

            if (user == null || role == null)
                return false;

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
                await _ctx.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var user = await _ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
            var role = await _ctx.Roles.FindAsync(roleId);

            if (user == null || role == null)
                return false;

            if (user.Roles.Contains(role))
            {
                user.Roles.Remove(role);
                await _ctx.SaveChangesAsync();
            }

            return true;
        }
    }

}
