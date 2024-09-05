using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IDjService
    {
        Task<Dj> CreateDjAsync(Dj dj);
        Task<Dj> GetDjByIdAsync(int id);
        Task<List<Dj>> GetAllDjAsync();
        Task<Dj> UpdateDjAsync(Dj dj);
        Task<bool> DeleteDjAsync(int id);
    }
}
