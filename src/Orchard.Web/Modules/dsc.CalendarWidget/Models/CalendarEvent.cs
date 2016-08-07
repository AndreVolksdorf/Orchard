using System;

namespace dsc.CalendarWidget.Models
{
    public class CalendarEvent
    {
        public string Title { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public bool AllDay { get; set; }
        public string Url { get; set; }
    }
}
