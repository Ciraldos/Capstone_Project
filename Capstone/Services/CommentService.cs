using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;


namespace Capstone.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _ctx;
        public CommentService(DataContext dataContext)
        {
            _ctx = dataContext;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment, int userId)
        {


            comment.UserId = userId;


            await _ctx.Comments.AddAsync(comment);
            await _ctx.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> ReplyCommentAsync(Comment comment, int userId, int parentCommentId)
        {

            comment.UserId = userId;
            comment.ParentCommentId = parentCommentId;
            await _ctx.Comments.AddAsync(comment);
            await _ctx.SaveChangesAsync();
            return comment;
        }
    }
}
