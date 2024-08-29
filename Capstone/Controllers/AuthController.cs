using Capstone.Context;
using Capstone.Models.ViewModels.Auth;
using Capstone.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly DataContext _dataContext;
        public AuthController(IAuthService authService, DataContext dataContext)
        {
            _authService = authService;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var existingUser = await _authService.LoginAsync(model);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existingUser.Username),
                    new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString())
                };

                existingUser.Roles.ForEach(r =>
                {
                    claims.Add(new Claim(ClaimTypes.Role, r.RoleName));
                });

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Tentativo di login fallito: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Register()
        {
            var genres = await _dataContext.Genres.ToListAsync();
            ViewBag.Genres = genres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _authService.RegisterAsync(model);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Registrazione fallita: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
