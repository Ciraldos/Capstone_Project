using Capstone.Context;
using Capstone.Models.ViewModels;
using Capstone.Models.ViewModels.Profile;
using Capstone.Services.Interfaces;
using Capstone.Services.Interfaces.Auth;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProfileViewModel = Capstone.Models.ViewModels.Profile.ProfileViewModel;

namespace Capstone.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userSvc;
        private IPasswordHelper _passwordHelper;
        private DataContext _ctx;
        private readonly IGenreService _genreService;
        public UserController(IUserService userService, IPasswordHelper passwordHelper, DataContext dataContext, IGenreService genreService)
        {
            _userSvc = userService;
            _passwordHelper = passwordHelper;
            _ctx = dataContext;
            _genreService = genreService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _userSvc.GetUserByIdAsync(userId);

                var availableGenres = await _genreService.GetAllGenresAsync();

                var viewModel = new ProfileViewModel
                {
                    User = user,
                    OldPassword = "",
                    NewPassword = "",
                    NewEmail = "",
                    AvailableGenres = availableGenres.ToList(), // Ensure this is populated
                    SelectedGenreIds = user.Genres.Select(g => g.GenreId).ToList() // Ensure this is populated
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: User/UpdateFavoriteGenres
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFavoriteGenres(ProfileViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                ModelState.AddModelError("", "Impossibile recuperare l'ID utente.");
                return View("Profile", model);
            }

            await _userSvc.UpdateUserGenresAsync(userId, model.SelectedGenreIds);
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                ModelState.AddModelError("", "Impossibile recuperare l'ID utente.");
                return View("Profile", new ProfileViewModel { OldPassword = model.OldPassword, NewPassword = model.NewPassword });
            }

            var result = await _userSvc.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                ModelState.AddModelError("", "Impossibile recuperare l'ID utente.");
                return View("Profile");
            }

            var result = await _userSvc.ChangeEmailAsync(userId, model.NewEmail);

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileImage(IFormFile imageFile)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                ModelState.AddModelError("", "Impossibile recuperare l'ID utente.");
                return RedirectToAction("Profile");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    var result = await _userSvc.UpdateUserProfileImageAsync(userId, memoryStream);
                }
            }

            return RedirectToAction("Profile");
        }
    }
}
