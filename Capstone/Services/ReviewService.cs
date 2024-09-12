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
        public async Task<Review> GetReviewByEventIdAndUserIdAsync(int eventId, int userId)
        {
            var review = await _ctx.Reviews
             .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
            return review!;
        }
        public async Task<IEnumerable<Review>> GetReviewsByEventIdAsync(int eventId)
        {
            return await _ctx.Reviews
                .Where(r => r.EventId == eventId)
                .Include(r => r.User) // Include the user for displaying the reviewer's name
                .Include(r => r.ReviewImgs) // Include images
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review reviewModel, List<IFormFile> imageFiles)
        {
            // Aggiungi la recensione al database e salva le modifiche per ottenere l'ID
            _ctx.Reviews.Add(reviewModel);
            await _ctx.SaveChangesAsync();

            // Ora che la recensione ha un ID, crea la cartella per le immagini
            var reviewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews", reviewModel.ReviewId.ToString());

            if (!Directory.Exists(reviewDirectory))
            {
                Directory.CreateDirectory(reviewDirectory);
            }

            // Gestisci le immagini con il percorso corretto
            var reviewImages = await HandleReviewImagesAsync(imageFiles, reviewModel);
            reviewModel.ReviewImgs = reviewImages;

            // Salva nuovamente per aggiornare i percorsi delle immagini nel database
            _ctx.Reviews.Update(reviewModel);
            await _ctx.SaveChangesAsync();

            return reviewModel;
        }


        public async Task<Review> UpdateReviewAsync(Review reviewModel, List<IFormFile> imageFiles)
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

            // Handle replacing existing images
            if (imageFiles != null && imageFiles.Any())
            {
                var reviewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews", reviewModel.ReviewId.ToString());

                foreach (var img in existingReview.ReviewImgs)
                {

                    // Remove 'wwwroot' from the beginning of the path for file deletion
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    Console.WriteLine($"Attempting to delete file: {imagePath}");

                    if (File.Exists(imagePath))
                    {
                        try
                        {
                            File.Delete(imagePath);
                            Console.WriteLine($"Successfully deleted file: {imagePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {imagePath}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {imagePath}");
                    }
                }

                _ctx.ReviewImgs.RemoveRange(existingReview.ReviewImgs);

                var replacementImages = await HandleReviewImagesAsync(imageFiles, existingReview);
                existingReview.ReviewImgs.AddRange(replacementImages);
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

            var reviewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews", reviewId.ToString());

            // Delete images from file system
            foreach (var img in review.ReviewImgs)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            // Delete the directory after images are removed
            if (Directory.Exists(reviewDirectory))
            {
                Directory.Delete(reviewDirectory, true);
            }

            // Remove the images and review from the database
            _ctx.ReviewImgs.RemoveRange(review.ReviewImgs);
            _ctx.Reviews.Remove(review);
            await _ctx.SaveChangesAsync();
            return true;
        }

        private async Task<List<ReviewImg>> HandleReviewImagesAsync(List<IFormFile> imageFiles, Review review)
        {
            var reviewImages = new List<ReviewImg>();
            var reviewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews", review.ReviewId.ToString());

            if (!Directory.Exists(reviewDirectory))
            {
                Directory.CreateDirectory(reviewDirectory);
            }

            foreach (var file in imageFiles)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(reviewDirectory, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Save the relative path to be used in the URL
                    var relativeFilePath = $"/uploads/reviews/{review.ReviewId}/{fileName}";

                    var reviewImg = new ReviewImg
                    {
                        Review = review,
                        FilePath = relativeFilePath
                    };
                    reviewImages.Add(reviewImg);
                }
            }

            return reviewImages;
        }

    }
}
