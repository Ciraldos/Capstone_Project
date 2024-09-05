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

        // GET: Genre
        public async Task<ActionResult> List()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return View(genres);
        }

        //todo: non penso verrà usata, eliminabile
        // GET: Genre/Details/5
        //public async Task<ActionResult> Details(int id)
        //{
        //    var genre = await _genreService.GetGenreByIdAsync(id);

        //    return View(genre);
        //}

        // GET: Genre/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreService.CreateGenreAsync(genre);
                return RedirectToAction("List");
            }
            return View(genre);
        }

        // GET: Genre/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);


            return View(genre);
        }

        // POST: Genre/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Genre genre)
        {
            if (ModelState.IsValid)
            {
                var updatedGenre = await _genreService.UpdateGenreAsync(genre);

                return RedirectToAction("List");
            }
            return View(genre);
        }

        // GET: Genre/Delete/5
        public async Task<ActionResult> Delete(int id, bool confirm = false)
        {
            if (confirm)
            {
                await _genreService.DeleteGenreAsync(id);
                return RedirectToAction("List");
            }

            var genreToDelete = await _genreService.GetGenreByIdAsync(id);


            return View(genreToDelete);
        }
    }
}
