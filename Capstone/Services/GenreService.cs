using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class GenreService : IGenreService
    {
        private readonly DataContext _ctx;

        public GenreService(DataContext ctx)
        {
            _ctx = ctx;
        }

        // Read all genres
        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _ctx.Genres.ToListAsync();
        }

        // Read a specific genre by ID
        public async Task<Genre> GetGenreByIdAsync(int genreId)
        {
            return await _ctx.Genres.FirstOrDefaultAsync(g => g.GenreId == genreId);
        }

        // Create a new genre
        public async Task<Genre> CreateGenreAsync(Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre));
            }

            _ctx.Genres.Add(genre);
            await _ctx.SaveChangesAsync();
            return genre;
        }

        // Update an existing genre
        public async Task<Genre> UpdateGenreAsync(Genre genre)
        {
            var existingGenre = await _ctx.Genres.FindAsync(genre.GenreId);
            if (existingGenre == null)
            {
                return null;
            }

            existingGenre.Name = genre.Name;

            await _ctx.SaveChangesAsync();
            return existingGenre;
        }

        // Delete a genre by its ID
        public async Task<bool> DeleteGenreAsync(int genreId)
        {
            var genre = await _ctx.Genres.FindAsync(genreId);
            if (genre == null)
            {
                return false;
            }

            _ctx.Genres.Remove(genre);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
