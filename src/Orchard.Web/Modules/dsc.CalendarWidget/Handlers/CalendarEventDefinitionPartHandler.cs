using dsc.CalendarWidget.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dsc.CalendarWidget.Handlers
{
    public class CalendarEventDefinitionPartHandler : ContentHandler
    {
        public CalendarEventDefinitionPartHandler(IRepository<CalendarEventDefinitionRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}