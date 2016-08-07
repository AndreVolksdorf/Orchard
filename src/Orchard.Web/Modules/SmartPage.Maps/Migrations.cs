using System.Data;
using SmartPage.Maps.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Widgets.Models;

namespace SmartPage.Maps
{
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            // Creating table MapRecord
            SchemaBuilder.CreateTable("MapRecord", table => table
                .ContentPartRecord()
                .Column("Latitude", DbType.Double)
                .Column("Longitude", DbType.Double)
                .Column("Street", DbType.String)
                .Column("StreetNumber", DbType.String)
                .Column("City", DbType.String)
                .Column("PostalCode", DbType.String)
                .Column("Area", DbType.String)
                .Column("Country", DbType.String)
            );

            ContentDefinitionManager.AlterPartDefinition(
                typeof(MapPart).Name, cfg => cfg.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("MapRecord", command => command.AddColumn<string>("Address"));

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterTypeDefinition(
              "MapMarkersWidget", cfg => cfg
                .WithSetting("Stereotype", "Widget")
                .WithPart(typeof(MapMarkersPart).Name)
                .WithPart(typeof(CommonPart).Name)
                .WithPart(typeof(WidgetPart).Name));

            SchemaBuilder.CreateTable(typeof(MapMarkersPartRecord).Name,
              table => table
                .ContentPartRecord()
                .Column<bool>("ScanContainedItems")
                .Column<int>("Height"));

            return 3;
        }

        public int UpdateFrom3()
        {
            ContentDefinitionManager.AlterTypeDefinition(
              "GroupMapMarkersWidget", cfg => cfg
                .WithSetting("Stereotype", "Widget")
                .WithPart(typeof(GroupMapMarkersPart).Name)
                .WithPart(typeof(CommonPart).Name)
                .WithPart(typeof(WidgetPart).Name));

            SchemaBuilder.CreateTable(typeof(GroupMapMarkersPartRecord).Name,
              table => table
                .ContentPartRecord()
                .Column<bool>("ScanContainedItems")
                .Column<int>("Height"));

            return 7;
        }

        public int UpdateFrom4()
        {

            // Creating table MapRecord
            SchemaBuilder.AlterTable("MapRecord", table =>
            {
                table.AddColumn("Street", DbType.String);
                table.AddColumn("City", DbType.String);
            });
            return 7;
        }

        public int UpdateFrom5()
        {

            // Creating table MapRecord
            SchemaBuilder.AlterTable("MapRecord", table =>
            {
                table.DropColumn("Phone");
                table.DropColumn("Email");
                table.DropColumn("Website");
                table.AddColumn("PostalCode", DbType.String);
                table.AddColumn("StreetNumber", DbType.String);
            });
            return 6;
        }

        public int UpdateFrom6()
        {

            // Creating table MapRecord
            SchemaBuilder.AlterTable("MapRecord", table =>
            {
                table.DropColumn("Address");
                table.AddColumn("Area", DbType.String);
                table.AddColumn("Country", DbType.String);
            });
            return 7;
        }
        public int UpdateFrom7()
        {

            // Creating table MapRecord
            SchemaBuilder.AlterTable("MapRecord", table =>
            {
                table.AddColumn("Name", DbType.String);
            });
            return 8;
        }
    }
}