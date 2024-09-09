﻿using Capstone.Context;
using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly DataContext _dataContext;

        public CommentLikeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<(int likeCount, List<string> likes)> GetCommentLikesAsync(int commentId)
        {
            var comment = await _dataContext.Comments
                .Include(c => c.CommentLikes)
                .ThenInclude(cl => cl.User)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (comment == null)
            {
                return (0, new List<string>());
            }

            var likeCount = comment.CommentLikes.Count;
            var likes = comment.CommentLikes.Select(cl => cl.User.Username).ToList();

            return (likeCount, likes);
        }


        public async Task<bool> ToggleLikeAsync(int commentId, int userId)
        {
            try
            {
                var comment = await _dataContext.Comments
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
                    _dataContext.CommentLikes.Remove(existingLike);
                }
                else
                {
                    var user = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
                    comment.CommentLikes.Add(new CommentLike { Comment = comment, User = user });
                }

                await _dataContext.SaveChangesAsync();
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
