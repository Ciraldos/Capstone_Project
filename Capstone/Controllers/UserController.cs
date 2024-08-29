using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userSvc;

        public UserController(IUserService userService)
        {
            _userSvc = userService;
        }

        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _userSvc.GetUserByIdAsync(userId);
                return View(user);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
