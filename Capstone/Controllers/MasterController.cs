using Capstone.Models;
using Capstone.Models.ViewModels;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    // [Authorize(Policy = "MasterPolicy")]
    public class MasterController : Controller
    {
        private readonly IMasterService _masterSvc;
        private readonly IUserService _userSvc;
        private readonly IRoleService _roleSvc;

        public MasterController(IMasterService masterService, IUserService userService, IRoleService roleSvc)
        {
            _masterSvc = masterService;
            _userSvc = userService;
            _roleSvc = roleSvc;
        }

        public async Task<IActionResult> MasterDashboard()
        {
            var users = await _userSvc.GetAllUsersAsync();
            var roles = await _roleSvc.GetAllRolesAsync();

            var viewModel = new MasterViewModel
            {
                Users = users,
                Roles = roles,
                NewRole = new Role { RoleName = string.Empty }
            };

            return View(viewModel);
        }

        // Delete User
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userSvc.DeleteUserAsync(id);
            return RedirectToAction("MasterDashboard");
        }

        // Create Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("MasterDashboard");
            }

            await _roleSvc.CreateRoleAsync(role);
            return RedirectToAction("MasterDashboard");
        }

        // Update Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            await _roleSvc.UpdateRoleAsync(role);
            return RedirectToAction("MasterDashboard");
        }

        // Delete Role
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleSvc.DeleteRoleAsync(id);
            return RedirectToAction("MasterDashboard");
        }

        // Add Role to User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleToUser(int userId, int roleId)
        {
            await _masterSvc.AddRoleToUserAsync(userId, roleId);
            return RedirectToAction("MasterDashboard");
        }

        // Remove Role from User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            await _masterSvc.RemoveRoleFromUserAsync(userId, roleId);
            return RedirectToAction("MasterDashboard");
        }
    }
}
