using dsc.CalendarWidget.Models;
using Microsoft.CSharp.RuntimeBinder;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dsc.CalendarWidget.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IProjectionManager _projectionManager;
        private readonly IOrchardServices _orchardServices;
        private readonly HttpContextBase _context;

        public CalendarService(IProjectionManager projectionManager, IOrchardServices orchardServices, HttpContextBase context)
        {
            _projectionManager = projectionManager;
            _orchardServices = orchardServices;
            _context = context;
        }

        public List<QueryPart> GetCalendarQueries()
        {
            IEnumerable<QueryPart> queryParts = _orchardServices.ContentManager.Query<QueryPart, QueryPartRecord>("Query").List();

            List<QueryPart> calendarQueries = new List<QueryPart>();

            foreach (QueryPart part in queryParts)
            {
                ContentItem contentItem = _projectionManager.GetContentItems(part.Id).FirstOrDefault();

                int countTitleParts = contentItem.TypeDefinition.Parts.Where(r => r.PartDefinition.Name == "TitlePart").Count();
                int countTimeSpanParts = contentItem.TypeDefinition.Parts.Where(r => r.PartDefinition.Name == "TimeSpanPart").Count();
                int countCalendarEventDefinition = contentItem.TypeDefinition.Parts.Where(r => r.PartDefinition.Name == "CalendarEventDefinition").Count();

                if ((countTitleParts > 0) && ((countTimeSpanParts > 0) || (countCalendarEventDefinition > 0)))
                {
                    calendarQueries.Add(part);
                }
            }

            return calendarQueries;
        }

        public List<CalendarEventInstance> GetCalendarEvents(CalendarWidgetPart part)
        {
            IEnumerable<ContentItem> contentItems = _projectionManager.GetContentItems(part.QueryId);

            List<CalendarEventInstance> calendarEvents = new List<CalendarEventInstance>();

            foreach (ContentItem item in contentItems)
            {
                dynamic record = _orchardServices.ContentManager.Get(item.Record.Id);

                CalendarEventInstance calendarEvent = new CalendarEventInstance
                {
                    Title = record.TitlePart.Title
                };

                // TimeSpan
                try{
                    var r = record.TimeSpanPart;
                    calendarEvent.Start = r.StartDateTime.DateTime;
                    calendarEvent.End = r.EndDateTime.DateTime;
                    calendarEvent.Url = String.Format("Admin/Contents/{0}", record.Id);
                    calendarEvent.AllDay = r.AllDay.Value;
                }
                catch (RuntimeBinderException)
                {
                }

                // CalendarEventDefinitionPart
                try
                {
                    dynamic d = record.CalendarEventDefinition;
                    CalendarEventDefinition r = d as CalendarEventDefinition;
                    if (!string.IsNullOrWhiteSpace(r.TimeZone))
                    {
                        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(r.TimeZone);
                        if (r.StartDateTime.HasValue) { calendarEvent.Start = new DateTimeOffset(r.StartDateTime.Value, tzi.GetUtcOffset(r.StartDateTime.Value)); }
                        if (r.EndDateTime.HasValue) { calendarEvent.End = new DateTimeOffset(r.EndDateTime.Value, tzi.GetUtcOffset(r.EndDateTime.Value)); }
                    }
                    else
                    {
                        if (r.StartDateTime.HasValue) { calendarEvent.Start = r.StartDateTime.Value; }
                        if (r.EndDateTime.HasValue) { calendarEvent.End = r.EndDateTime.Value; }
                    }

                    if (r.StartDateTime.HasValue)
                    {
                        r.StartDate = r.StartDateTime.Value.ToString("d", _context.Response.Output.FormatProvider);
                        r.StartTime = r.StartDateTime.Value.ToString("t", _context.Response.Output.FormatProvider);
                    }

                    if (r.EndDateTime.HasValue)
                    {
                        r.EndDate = r.EndDateTime.Value.ToString("d", _context.Response.Output.FormatProvider);
                        r.EndTime = r.EndDateTime.Value.ToString("t", _context.Response.Output.FormatProvider);
                    }

                    if (r.Url.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
                    {
                        calendarEvent.Url = string.Format("Admin/Contents/Edit/{0}", item.Id);
                    }
                    else if (r.Url.Equals("view", StringComparison.InvariantCultureIgnoreCase))
                    {
                        calendarEvent.Url = string.Format("Contents/Item/Display/{0}", item.Id);
                    }
                    else
                    {
                        calendarEvent.Url = r.Url;
                    }
                    calendarEvent.AllDay = r.IsAllDay;
                    //calendarEvent.IsRecurring = r.IsRecurring;
                }
                catch (RuntimeBinderException)
                {
                }

                calendarEvents.Add(calendarEvent);
            }

            return calendarEvents;
        }
    }
}