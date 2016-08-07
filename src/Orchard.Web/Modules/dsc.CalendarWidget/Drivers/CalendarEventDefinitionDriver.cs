using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dsc.CalendarWidget.Models;
using Orchard.Time;
using Orchard;
using System.Globalization;

namespace dsc.CalendarWidget.Drivers
{
    public class CalendarEventDefinitionDriver : ContentPartDriver<CalendarEventDefinition>
    {
        private const string TemplateName = "Parts/CalendarEventDefinition";
        private IWorkContextAccessor _workContextAccessor;
        private HttpContextBase _context;

        public Localizer T { get; set; }

        public CalendarEventDefinitionDriver(HttpContextBase context, IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _context = context;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix
        {
            get { return "CalendarEventDefinition"; }
        }

        protected override DriverResult Display(CalendarEventDefinition part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_CalendarEventDefinition",
                    () => shapeHelper.Parts_CalendarEventDefinition(part)),
                ContentShape("Parts_CalendarEventDefinition_Summary",
                    () => shapeHelper.Parts_CalendarEventDefinition_Summary(part)),
                ContentShape("Parts_CalendarEventDefinition_SummaryAdmin",
                    () => shapeHelper.Parts_CalendarEventDefinition_SummaryAdmin(part))
                );
        }

        protected override DriverResult Editor(CalendarEventDefinition part, dynamic shapeHelper)
        {
            SiteTimeZoneSelector tz = new SiteTimeZoneSelector(_workContextAccessor);
            CultureInfo ci = new CultureInfo(_workContextAccessor.GetContext().CurrentCulture);
            //part.TimeZone=tz.GetTimeZone(_context).TimeZone.Id;

            if (part.StartDateTime.HasValue)
            {
                part.StartDate = part.StartDateTime.Value.ToString("d", ci);
                part.StartTime = part.StartDateTime.Value.ToString("t", ci);
            }
            if (part.EndDateTime.HasValue)
            {
                part.EndDate = part.EndDateTime.Value.ToString("d", ci);
                part.EndTime = part.EndDateTime.Value.ToString("t", ci);
            }
            return ContentShape("Parts_CalendarEventDefinition_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CalendarEventDefinition part, IUpdateModel updater, dynamic shapeHelper)
        {
            DateTime d;
            TimeSpan t;
            CultureInfo ci = new CultureInfo(_workContextAccessor.GetContext().CurrentCulture);

            if (!string.IsNullOrWhiteSpace(part.StartDate))
            {
                if (DateTime.TryParse(part.StartDate, ci, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out d))
                {
                    part.StartDateTime = d + part.StartDateTime.Value.TimeOfDay;
                }
            }

            if (!string.IsNullOrWhiteSpace(part.StartTime))
            {
                if (TimeSpan.TryParse(part.StartTime, ci, out t))
                {
                    part.StartDateTime = part.StartDateTime.Value.Date + t;
                }
            }

            if (!string.IsNullOrWhiteSpace(part.EndDate))
            {
                if (DateTime.TryParse(part.EndDate, ci, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out d))
                {
                    part.EndDateTime = d + part.EndDateTime.Value.TimeOfDay;
                }
            }

            if (!string.IsNullOrWhiteSpace(part.EndTime))
            {
                if (TimeSpan.TryParse(part.EndTime, ci, out t))
                {
                    part.EndDateTime = part.EndDateTime.Value.Date + t;
                }
            }

            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Importing(CalendarEventDefinition part, ImportContentContext context)
        {
            base.Importing(part, context);

            throw new NotImplementedException();
        }
        protected override void Exporting(CalendarEventDefinition part, ExportContentContext context)
        {
            base.Exporting(part, context);

            throw new NotImplementedException();
        }
    }
}