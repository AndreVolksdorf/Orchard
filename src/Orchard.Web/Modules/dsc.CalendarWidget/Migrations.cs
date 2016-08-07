using Orchard.Data.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using dsc.CalendarWidget.Models;
using Orchard.ContentManagement.MetaData.Models;
using System.Data;

namespace dsc.CalendarWidget
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            //ContentDefinitionManager.AlterPartDefinition("TimeSpanPart",
            //    builder => builder
            //        .Attachable()
            //        .WithField("StartDateTime",
            //            field => field
            //                .OfType("DateTimeField")
            //                .WithDisplayName("Start DateTime")
            //        )
            //        .WithField("EndDateTime",
            //            field => field
            //                .OfType("DateTimeField")
            //                .WithDisplayName("End DateTime")
            //        )
            //        .WithField("AllDay",
            //            field=>field
            //                    .OfType("BooleanField")
            //                    .WithDisplayName("AllDay")
            //        )
            //);

            //SchemaBuilder.CreateTable("CalendarEventDefinitionPartRecord",
            //    table => table
            //        .ContentPartRecord()
            //        .Column<string>("TimeZone")
            //        .Column<DateTime>("StartDateTime")
            //        .Column<DateTime>("EndDateTime")
            //        .Column<bool>("IsAllDay")
            //        .Column<string>("Url")
            //        .Column<bool>("IsRecurring")
            //);

            //ContentDefinitionManager.AlterPartDefinition("CalendarEventDefinitionPart",
            //    builder => builder
            //        .Attachable()
            //        .WithDescription("Provides calendar event settings to your content. Use this for displaying events in Calendar Widget")
            //);

            SchemaBuilder.CreateTable("CalendarWidgetPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("QueryId")
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(CalendarWidgetPart).Name,
                builder => builder
                    .Attachable()
                    .WithDescription("The calendar widget part"));

            ContentDefinitionManager.AlterTypeDefinition("CalendarWidget",
                cfg => cfg
                    .WithPart("CalendarWidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                    );

            return 2;
        }

        //public int UpdateFrom1()
        //{
        //    ContentDefinitionManager.AlterPartDefinition("TimeSpanPart",
        //        builder => builder
        //            .WithField("AllDay",
        //                field => field
        //                    .OfType("BooleanField")
        //                    .WithDisplayName("AllDay")
        //            )
        //        );
        //    return 2;
        //}

        public int UpdateFrom2()
        {
            SchemaBuilder.CreateTable("CalendarEventDefinitionRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("TimeZone")
                    .Column<DateTime>("StartDateTime")
                    .Column<DateTime>("EndDateTime")
                    .Column<bool>("IsAllDay")
                    .Column<string>("Url")
                    .Column<bool>("IsRecurring")
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(CalendarEventDefinition).Name,
                builder => builder
                    .Attachable()
                    .WithDescription("Provides calendar event settings to your content. Use this for displaying events in Calendar Widget")
            );

            return 3;
        }
    }
}