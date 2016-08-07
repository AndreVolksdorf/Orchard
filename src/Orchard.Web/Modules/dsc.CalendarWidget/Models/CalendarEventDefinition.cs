using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace dsc.CalendarWidget.Models
{
    public class CalendarEventDefinition : ContentPart<CalendarEventDefinitionRecord>
    {
        public string TimeZone {
            get { return Record.TimeZone; }
            set { Record.TimeZone = value; }
        }

        public DateTime? StartDateTime
        {
            get { return Record.StartDateTime; }
            set { Record.StartDateTime = value; }
        }

        //public string StartDate {
        //    get { return Record.StartDateTime.HasValue ? Record.StartDateTime.Value.ToShortDateString() : ""; }
        //    set
        //    {
        //        DateTime sdt;
        //        if (Record.StartDateTime.HasValue) { sdt = Record.StartDateTime.Value; }
        //        else { sdt = DateTime.MinValue; }

        //        Record.StartDateTime = DateTime.Parse(value) + sdt.TimeOfDay;
        //    }
        //}

        //public string StartTime {
        //    get { return Record.StartDateTime.HasValue ? Record.StartDateTime.Value.ToShortTimeString() : ""; }
        //    set
        //    {
        //        DateTime sdt;
        //        if (Record.StartDateTime.HasValue) { sdt = Record.StartDateTime.Value; }
        //        else { sdt = DateTime.MinValue; }

        //        Record.StartDateTime = sdt.Date + TimeSpan.Parse(value);
        //    }
        //}

        public string StartDate { get; set; }
        public string StartTime { get; set; }

        public bool ShowStartDate
        {
            get { return true; }
        }

        public bool ShowStartTime
        {
            get { return !IsAllDay; }
        }

        public DateTime? EndDateTime
        {
            get { return Record.EndDateTime; }
            set { Record.EndDateTime = value; }
        }

        //public string EndDate {
        //    get { return Record.EndDateTime.HasValue ? Record.EndDateTime.Value.ToShortDateString() : ""; }
        //    set
        //    {
        //        DateTime sdt;
        //        if (Record.EndDateTime.HasValue) { sdt = Record.EndDateTime.Value; }
        //        else { sdt = DateTime.MinValue; }

        //        Record.EndDateTime = DateTime.Parse(value) + sdt.TimeOfDay;
        //    }
        //}

        //public string EndTime {
        //    get { return Record.EndDateTime.HasValue ? Record.EndDateTime.Value.ToShortTimeString() : ""; }
        //    set
        //    {
        //        DateTime sdt;
        //        if (Record.EndDateTime.HasValue) { sdt = Record.EndDateTime.Value; }
        //        else { sdt = DateTime.MinValue; }

        //        Record.EndDateTime = sdt.Date + TimeSpan.Parse(value);
        //    }
        //}

        public string EndDate { get; set; }
        public string EndTime { get; set; }

        public bool ShowEndDate
        {
            get { return true; }
        }

        public bool ShowEndTime
        {
            get { return !IsAllDay; }
        }

        public bool IsAllDay
        {
            get { return Record.IsAllDay; }
            set { Record.IsAllDay = value; }
        }

        public string Url
        {
            get { return Record.Url; }
            set { Record.Url = value; }
        }

        public bool IsRecurring
        {
            get { return Record.IsRecurring; }
            set { Record.IsRecurring = value; }
        }

        //internal CalendarEventDefinitionPart Copy()
        //{
        //    return new CalendarEventDefinitionPart()
        //    {
        //        TimeZone = TimeZone,
        //        StartDateTime = StartDateTime.GetValueOrDefault(),
        //        EndDateTime = EndDateTime.GetValueOrDefault(),
        //        IsAllDay = IsAllDay,
        //        Url = Url,
        //        IsRecurring = IsRecurring
        //    };
        //}
    }
}
