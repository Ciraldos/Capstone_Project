using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId);
        Task<Review> CreateReviewAsync(Review reviewModel, List<IFormFile> imageFiles);
        Task<Review> UpdateReviewAsync(Review reviewModel, List<IFormFile> imageFiles);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<Review> GetReviewByEventIdAndUserIdAsync(int eventId, int userId);
    }
}
