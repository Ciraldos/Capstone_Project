using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;

namespace Capstone.Services
{
    public class GenreService : IGenreService
    {
        public DataContext _ctx;
        public GenreService(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Genre> CreateGenre(Genre genre)
        {
            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre));
            }

            _ctx.Genres.Add(genre); // Assuming `Genres` is the DbSet<Genre>
            await _ctx.SaveChangesAsync();
            return genre;
        }
    }
}
