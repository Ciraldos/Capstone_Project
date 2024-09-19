using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly DataContext _ctx;

        public CommentLikeService(DataContext dataContext)
        {
            _ctx = dataContext;
        }
        public async Task<(int likeCount, IEnumerable<User> likes, bool userHasLiked)> GetCommentLikesAsync(int commentId, int userId)
        {
            var comment = await _ctx.Comments
                .Include(c => c.CommentLikes)
                .ThenInclude(cl => cl.User)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                return (0, Enumerable.Empty<User>(), false);
            }

            var userHasLiked = comment.CommentLikes.Any(cl => cl.UserId == userId);
            var likeCount = comment.CommentLikes.Count;
            var likes = comment.CommentLikes.Select(cl => cl.User);

            return (likeCount, likes, userHasLiked);
        }



        public async Task<bool> ToggleLikeAsync(int commentId, int userId)
        {
            try
            {
                var comment = await _ctx.Comments
                    .Include(c => c.CommentLikes)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return false;
                }

                var existingLike = comment.CommentLikes
                    .FirstOrDefault(cl => cl.UserId == userId);

                if (existingLike != null)
                {
                    _ctx.CommentLikes.Remove(existingLike);
                }
                else
                {
                    var user = _ctx.Users.FirstOrDefault(u => u.UserId == userId);
                    comment.CommentLikes.Add(new CommentLike { Comment = comment, User = user });
                }

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LikeReviewAsync: {ex.Message}");
                return false;
            }
        }
    }
}
