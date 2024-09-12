using Capstone.Models.AdminControllerModels;

namespace Capstone.Services.Interfaces
{
    public interface IAdminService
    {
        List<TicketSalesData> GetTicketSalesPerEvent();
        List<TicketSalesPerDayData> GetTicketSalesPerDay();
        int GetCommentsThisWeek();
        List<ReviewPerEventData> GetReviewsPerEvent();
        List<LikesPerCommentData> GetLikesPerComment();
        List<EventsPerDjData> GetEventsPerDj();
        List<TicketSalesByTypeData> GetTicketSalesByType();
        List<ReviewsPerEventTypeData> GetReviewsPerEventType();
        EventsStatusData GetEventsStatus();
        List<DjPopularityData> GetDjPopularity();




    }
}
