using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId);
        Task<Review> CreateReviewAsync(Review reviewModel, List<MemoryStream> imageStreams);
        Task<Review> UpdateReviewAsync(Review reviewModel, List<MemoryStream> replaceImageStreams, List<MemoryStream> additionalImageStreams);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}
