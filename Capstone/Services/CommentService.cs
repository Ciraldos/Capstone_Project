using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;


namespace Capstone.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _dataContext;
        public CommentService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Comment?> CreateCommentAsync(Comment comment, int userId)
        {
            if (comment == null)
            {
                return null;
            }

            comment.UserId = userId;
            comment.Like = 0; // Inizializza i likes a 0

            await _dataContext.Comments.AddAsync(comment);
            await _dataContext.SaveChangesAsync();

            return comment;
        }
    }
}
