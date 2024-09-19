using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ICommentLikeService
    {
        Task<bool> ToggleLikeAsync(int commentId, int userId);
        Task<(int likeCount, IEnumerable<User> likes, bool userHasLiked)> GetCommentLikesAsync(int commentId, int userId);



    }
}
