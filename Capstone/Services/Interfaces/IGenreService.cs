using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IGenreService
    {
        Task<Genre> CreateGenre(Genre genre);

    }
}
