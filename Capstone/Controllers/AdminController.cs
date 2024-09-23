using Capstone.Models.AdminControllerModels;
using Capstone.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminSvc;

        public AdminController(IAdminService adminService)
        {
            _adminSvc = adminService;
        }
        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                TicketSalesPerDay = _adminSvc.GetTicketSalesPerDay(),
                CommentsThisWeek = _adminSvc.GetCommentsThisWeek(),
                ReviewsPerEvent = _adminSvc.GetReviewsPerEvent(),
                LikesPerComment = _adminSvc.GetLikesPerComment(),
                EventsPerDj = _adminSvc.GetEventsPerDj(),
                TicketSalesByType = _adminSvc.GetTicketSalesByType(),
                ReviewsPerEventType = _adminSvc.GetReviewsPerEventType(),
                EventsStatus = _adminSvc.GetEventsStatus(),
                DjPopularity = _adminSvc.GetDjPopularity(),
            };

            return View(model);
        }
    }
}
