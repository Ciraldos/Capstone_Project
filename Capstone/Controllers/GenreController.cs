using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreSvc;

        public GenreController(IGenreService genreService)
        {
            _genreSvc = genreService;
        }

        // GET: Genre
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public async Task<ActionResult> List()
        {
            var genres = await _genreSvc.GetAllGenresAsync();
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
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public async Task<ActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreSvc.CreateGenreAsync(genre);
                return RedirectToAction("List");
            }
            return View(genre);
        }

        // GET: Genre/Edit/5
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public async Task<ActionResult> Edit(int id)
        {
            var genre = await _genreSvc.GetGenreByIdAsync(id);


            return View(genre);
        }

        // POST: Genre/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public async Task<ActionResult> Edit(Genre genre)
        {
            if (ModelState.IsValid)
            {
                var updatedGenre = await _genreSvc.UpdateGenreAsync(genre);

                return RedirectToAction("List");
            }
            return View(genre);
        }

        // GET: Genre/Delete/5
        [Authorize(Policy = "AdminOrMasterPolicy")]

        public async Task<ActionResult> Delete(int id, bool confirm = false)
        {
            if (confirm)
            {
                await _genreSvc.DeleteGenreAsync(id);
                return RedirectToAction("List");
            }

            var genreToDelete = await _genreSvc.GetGenreByIdAsync(id);


            return View(genreToDelete);
        }
    }
}
