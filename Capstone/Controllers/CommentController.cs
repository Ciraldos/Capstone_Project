using Capstone.Models;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
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
    }
}
