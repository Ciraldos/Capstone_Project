using Capstone.Context;
using Capstone.Models.AdminControllerModels;
using Capstone.Services.Interfaces;

public class AdminService : IAdminService
{
    private readonly DataContext _ctx;

    public AdminService(DataContext dataContext)
    {
        _ctx = dataContext;
    }

    public List<TicketSalesData> GetTicketSalesPerEvent()
    {
        return _ctx.Tickets
            .GroupBy(t => t.Event.Name) // Raggruppa i biglietti per evento
            .Select(g => new TicketSalesData
            {
                EventName = g.Key,       // Nome dell'evento
                TicketsSold = g.Count()  // Numero di biglietti venduti
            })
            .ToList();
    }
    public List<TicketSalesPerDayData> GetTicketSalesPerDay()
    {
        return _ctx.Tickets
            .GroupBy(t => t.PurchaseDate.Date)
            .Select(g => new TicketSalesPerDayData
            {
                Day = g.Key,
                TicketsSold = g.Count()
            })
            .ToList();
    }

    public int GetCommentsThisWeek()
    {
        var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
        return _ctx.Comments
            .Where(c => c.PublishedAt >= startOfWeek)
            .Count();
    }

    public List<ReviewPerEventData> GetReviewsPerEvent()
    {
        return _ctx.Reviews
            .GroupBy(r => r.Event.Name)
            .Select(g => new ReviewPerEventData
            {
                EventName = g.Key,
                ReviewsCount = g.Count()
            })
            .ToList();
    }

    public List<LikesPerCommentData> GetLikesPerComment()
    {
        return _ctx.CommentLikes
            .GroupBy(cl => cl.Comment.Description)
            .Select(g => new LikesPerCommentData
            {
                CommentDescription = g.Key,
                LikesCount = g.Count()
            })
            .ToList();
    }

    public List<EventsPerDjData> GetEventsPerDj()
    {
        return _ctx.Djs
            .Select(dj => new EventsPerDjData
            {
                DjName = dj.ArtistName,
                EventsCount = dj.Events.Count()
            })
            .ToList();
    }
    public List<TicketSalesByTypeData> GetTicketSalesByType()
    {
        return _ctx.Tickets
            .GroupBy(t => t.TicketType.TicketTypeName)
            .Select(g => new TicketSalesByTypeData
            {
                TicketTypeName = g.Key,
                TicketsSold = g.Count()
            })
            .ToList();
    }

    public List<ReviewsPerEventTypeData> GetReviewsPerEventType()
    {
        return _ctx.Reviews
            .GroupBy(r => r.Event.Genres.FirstOrDefault().Name)
            .Select(g => new ReviewsPerEventTypeData
            {
                EventType = g.Key ?? "Non specificato",
                ReviewsCount = g.Count()
            })
            .ToList();
    }

    public EventsStatusData GetEventsStatus()
    {
        var today = DateTime.Today;
        return new EventsStatusData
        {
            FutureEvents = _ctx.Events.Count(e => e.DateFrom >= today),
            PastEvents = _ctx.Events.Count(e => e.DateTo < today)
        };
    }

    public List<DjPopularityData> GetDjPopularity()
    {
        return _ctx.Djs
            .Select(dj => new DjPopularityData
            {
                DjName = dj.ArtistName,
                EventsCount = dj.Events.Count(),
                ReviewsCount = _ctx.Reviews.Count(r => r.Event.Djs.Contains(dj))
            })
            .ToList();
    }

}

