namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class CalendarEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Color { get; set; }
        public ExtendedProps ExtendedProps { get; set; }
    }
    public class ExtendedProps
    {
        public bool Status { get; set; }
        public string StylistName { get; set; }
    }
}
