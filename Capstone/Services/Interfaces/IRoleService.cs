using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}
