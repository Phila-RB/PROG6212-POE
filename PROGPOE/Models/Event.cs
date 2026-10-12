namespace PROGPOE.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public string EventName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; }
    }
}
