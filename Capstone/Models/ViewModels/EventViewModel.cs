namespace Capstone.Models.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        // Altre proprietà necessarie

    }
}
