namespace Capstone.Services.Interfaces
{
    public interface IMasterService
    {
        // Users


        // Roles


        // User-Role Management
        Task<bool> AddRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    }
}
