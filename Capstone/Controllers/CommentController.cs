using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ICommentLikeService _commentLikeService;

        public CommentController(ICommentService commentService, ICommentLikeService commentLikeService)
        {
            _commentService = commentService;
            _commentLikeService = commentLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            // Recupera l'ID dell'utente loggato (es. da User.Identity)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int.TryParse(userIdString, out int userId);
            var result = await _commentService.CreateCommentAsync(comment, userId);


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

            var result = await _commentLikeService.ToggleLikeAsync(id, userId);
            if (!result)
            {
                return NotFound(); // O puoi gestire diversamente a seconda della logica del tuo servizio
            }

            // Restituisci i dati aggiornati
            var (likeCount, likes) = await _commentLikeService.GetCommentLikesAsync(id);

            return Json(new { likeCount, likes });
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentLikes(int commentId)
        {
            var (likeCount, likes) = await _commentLikeService.GetCommentLikesAsync(commentId);

            return Json(new { likeCount, likes });
        }
    }
}
