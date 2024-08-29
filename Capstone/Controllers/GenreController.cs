using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }
        public IActionResult CreateGenre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(Genre genre)
        {

            await _genreService.CreateGenre(genre);
            return RedirectToAction("Index", "Home");
        }
    }
}
