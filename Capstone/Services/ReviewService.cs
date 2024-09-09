using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext _ctx;

        public ReviewService(DataContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId)
        {
            return await _ctx.Reviews
                .Where(r => r.EventId == eventId)
                .Include(r => r.User) // Include the user for displaying the reviewer's name
                .Include(r => r.ReviewImgs) // Include images
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review reviewModel, List<MemoryStream> imageStreams)
        {
            var eventObj = await _ctx.Events.FindAsync(reviewModel.EventId);

            reviewModel.Event = eventObj!;

            // Handle the image streams
            var reviewImages = await HandleReviewImagesAsync(imageStreams, reviewModel);
            reviewModel.ReviewImgs = reviewImages;

            _ctx.Reviews.Add(reviewModel);
            await _ctx.SaveChangesAsync();

            return reviewModel;
        }

        public async Task<Review> UpdateReviewAsync(Review reviewModel, List<MemoryStream> replaceImageStreams, List<MemoryStream> additionalImageStreams)
        {
            var existingReview = await _ctx.Reviews
                .Include(r => r.ReviewImgs)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewModel.ReviewId);

            if (existingReview == null)
            {
                throw new ArgumentException("Review not found.");
            }

            // Update review details
            existingReview.Rating = reviewModel.Rating;
            existingReview.Title = reviewModel.Title;
            existingReview.Description = reviewModel.Description;

            // Replace existing images if needed
            if (replaceImageStreams != null && replaceImageStreams.Any())
            {
                _ctx.ReviewImgs.RemoveRange(existingReview.ReviewImgs);
                var replacementImages = await HandleReviewImagesAsync(replaceImageStreams, existingReview);
                existingReview.ReviewImgs.AddRange(replacementImages);
            }

            // Add additional images
            if (additionalImageStreams != null && additionalImageStreams.Any())
            {
                var additionalImages = await HandleReviewImagesAsync(additionalImageStreams, existingReview);
                existingReview.ReviewImgs.AddRange(additionalImages);
            }

            await _ctx.SaveChangesAsync();
            return existingReview;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _ctx.Reviews
                .Include(r => r.ReviewImgs)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review == null)
            {
                return false;
            }

            _ctx.ReviewImgs.RemoveRange(review.ReviewImgs);
            _ctx.Reviews.Remove(review);
            await _ctx.SaveChangesAsync();
            return true;
        }

        private async Task<List<ReviewImg>> HandleReviewImagesAsync(List<MemoryStream> imageStreams, Review review)
        {
            var reviewImages = new List<ReviewImg>();

            foreach (var stream in imageStreams)
            {
                // Reset the stream position to the beginning
                stream.Position = 0;

                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var reviewImg = new ReviewImg
                    {
                        ImgData = memoryStream.ToArray(),  // Convert MemoryStream to byte array
                        Review = review
                    };
                    reviewImages.Add(reviewImg);
                }
            }

            return reviewImages;
        }
    }
}
