using Capstone.Models.ViewModels;
using Capstone.Models.ViewModels.Profile;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userSvc;
        private readonly IGenreService _genreSvc;
        public UserController(IUserService userService, IGenreService genreService)
        {
            _userSvc = userService;
            _genreSvc = genreService;
        }

        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _userSvc.GetUserByIdAsync(userId);

                // Load available genres
                var availableGenres = await _genreSvc.GetAllGenresAsync();

                // Store genres in ViewBag
                ViewBag.AvailableGenres = availableGenres.ToList(); // Ensure it's populated

                var viewModel = new ProfileViewModel
                {
                    User = user,
                    OldPassword = "",
                    NewPassword = "",
                    NewEmail = "",
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

            try
            {
                var result = await _userSvc.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                if (result)
                {
                    // Password changed successfully
                    return RedirectToAction("Profile");
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message); // Add the exception message to the model state
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message); // Handle other service exceptions
            }

            // If we reach here, something went wrong, return the view with error messages
            return View("Profile", new ProfileViewModel { OldPassword = model.OldPassword, NewPassword = model.NewPassword });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                ModelState.AddModelError("", "Impossibile recuperare l'ID utente.");
                return View("Profile", new ProfileViewModel { NewEmail = model.NewEmail });
            }

            try
            {
                var result = await _userSvc.ChangeEmailAsync(userId, model.NewEmail);
                if (result)
                {
                    // Email changed successfully
                    return RedirectToAction("Profile");
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message); // Add the exception message to the model state
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message); // Handle other service exceptions
            }

            // If we reach here, something went wrong, return the view with error messages
            return View("Profile", new ProfileViewModel { NewEmail = model.NewEmail });
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
