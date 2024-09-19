using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentSvc;
        private readonly ICommentLikeService _commentLikeSvc;

        public CommentController(ICommentService commentService, ICommentLikeService commentLikeService)
        {
            _commentSvc = commentService;
            _commentLikeSvc = commentLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            // Recupera l'ID dell'utente loggato (es. da User.Identity)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int.TryParse(userIdString, out int userId);
            var result = await _commentSvc.CreateCommentAsync(comment, userId);


            if (result != null)
            {
                return RedirectToAction("Details", "Event", new { id = comment.EventId });
            }

            // Se il commento non è valido, ritorna alla vista dell'evento con un messaggio di errore
            return RedirectToAction("Details", "Event", new { id = comment.EventId });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(Comment model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, error = "Unable to retrieve User ID." });
            }

            model.UserId = userId; // Assign the UserId to the comment

            var updatedcomment = await _commentSvc.UpdateCommentAsync(model);

            return Json(new
            {
                success = true,
                comment = new
                {
                    description = updatedcomment.Description,
                    gifUrl = updatedcomment.GifUrl
                }
            });
        }

        // POST: Review/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                await _commentSvc.DeleteCommentAsync(commentId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reply(Comment comment, int parentCommentId)
        {
            // Recupera l'ID dell'utente loggato (es. da User.Identity)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int.TryParse(userIdString, out int userId);
            var result = await _commentSvc.ReplyCommentAsync(comment, userId, parentCommentId);

            if (result != null)
            {
                return RedirectToAction("Details", "Event", new { id = comment.EventId });
            }

            // Se il commento non è valido, ritorna alla vista dell'evento con un messaggio di errore
            return RedirectToAction("Details", "Event", new { id = comment.EventId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdString, out int userId);

            var result = await _commentLikeSvc.ToggleLikeAsync(id, userId);
            if (!result)
            {
                return NotFound(); // O puoi gestire diversamente a seconda della logica del tuo servizio
            }

            // Restituisci i dati aggiornati
            var (likeCount, likes, userHasLiked) = await _commentLikeSvc.GetCommentLikesAsync(id, userId);

            return Json(new { likeCount, userHasLiked });
        }


        [HttpGet]
        public async Task<IActionResult> GetCommentLikes(int commentId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdString, out int userId);

            var (likeCount, likes, userHasLiked) = await _commentLikeSvc.GetCommentLikesAsync(commentId, userId);

            return Json(new { likeCount, likes, userHasLiked });
        }
    }
}
