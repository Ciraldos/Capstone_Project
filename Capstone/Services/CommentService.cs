using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


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

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _ctx.Comments
                .Include(c => c.CommentLikes)
                .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);

            if (existingComment == null)
            {
                throw new ArgumentException("Comment not found.");
            }

            // Update review details
            existingComment.Description = comment.Description;
            existingComment.GifUrl = comment.GifUrl;
            await _ctx.SaveChangesAsync();
            return existingComment;
        }





        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _ctx.Comments
                .Include(c => c.CommentLikes)
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                return false;
            }
            _ctx.CommentLikes.RemoveRange(comment.CommentLikes);

            // Elimina tutte le risposte associate a questo commento
            // Da decidere, io eviterei

            //_ctx.Comments.RemoveRange(comment.Replies);
            _ctx.Comments.Remove(comment);
            await _ctx.SaveChangesAsync();

            return true;
        }
    }
}
