using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Comment comment, int userId);
        Task<Comment> ReplyCommentAsync(Comment comment, int userId, int parentCommentId);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int commentId);

    }
}
