using Capstone.Models;

namespace Capstone.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment?> CreateCommentAsync(Comment comment, int userId);

    }
}
