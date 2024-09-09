namespace Capstone.Services.Interfaces
{
    public interface ICommentLikeService
    {
        Task<bool> ToggleLikeAsync(int commentId, int userId);
        Task<(int likeCount, List<string> likes)> GetCommentLikesAsync(int commentId);


    }
}
