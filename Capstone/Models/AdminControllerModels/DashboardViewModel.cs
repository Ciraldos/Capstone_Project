namespace Capstone.Models.AdminControllerModels
{
    public class DashboardViewModel
    {
        public List<TicketSalesPerDayData> TicketSalesPerDay { get; set; } = [];
        public int CommentsThisWeek { get; set; }
        public List<ReviewPerEventData> ReviewsPerEvent { get; set; } = [];
        public List<LikesPerCommentData> LikesPerComment { get; set; } = [];
        public List<EventsPerDjData> EventsPerDj { get; set; } = [];
        public List<TicketSalesByTypeData> TicketSalesByType { get; set; } = [];
        public List<ReviewsPerEventTypeData> ReviewsPerEventType { get; set; } = [];
        public EventsStatusData EventsStatus { get; set; }
        public List<DjPopularityData> DjPopularity { get; set; } = [];
    }
}
