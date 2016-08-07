using System;
using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace SmartPage.Community.Groups
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("GroupPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Description", column => column.Unlimited())
                    .Column<int>("DepartmentCount")
                    .Column<int>("PostCount")
                    .Column<bool>("DepartmentedPosts")
                    .Column<int>("Weight")
                    .Column<int>("GroupRecord_id")
                );

            SchemaBuilder.CreateTable("DepartmentPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("PostCount")
                    .Column<bool>("IsSticky")
                    .Column<int>("ClosedById")
                    .Column<DateTime>("ClosedOnUtc", column => column.WithDefault(null))
                    .Column<string>("ClosedDescription", column => column.Unlimited())
                );

            SchemaBuilder.CreateTable("GroupPostPartRecord",
                table => table
                    .ContentPartVersionRecord()
                    .Column<int>("RepliedOn", column => column.WithDefault(null))
                    .Column<string>("Text", column => column.Unlimited())
                    .Column<string>("Format")
                );

            ContentDefinitionManager.AlterPartDefinition("GroupPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Group Type with a hierarchy of different departments/posts"));

            ContentDefinitionManager.AlterPartDefinition("DepartmentPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Department Type, useful when wanting different types of departments for different groups"));

            ContentDefinitionManager.AlterPartDefinition("GroupPostPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Post Type, useful when wanting different types of posts for different groups"));

            ContentDefinitionManager.AlterTypeDefinition("Group", cfg => cfg
                .WithPart("GroupPart")
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'groups/my-group'}]")
                    .WithSetting("AutorouteSettings.PerItemConfiguration", "false")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
            );

            ContentDefinitionManager.AlterTypeDefinition("Department", cfg => cfg
                .WithPart("DepartmentPart")
                .WithPart("CommonPart", builder => builder
                    .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Group and Title', Pattern: '{Content.Container.Path}/{Content.Slug}', Description: 'groups/my-group/my-department'}]")
                    .WithSetting("AutorouteSettings.PerItemConfiguration", "false")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
            );

            ContentDefinitionManager.AlterTypeDefinition("GroupPost", cfg => cfg
                .WithPart("GroupPostPart")
                .WithPart("CommonPart", builder => builder
                    .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Group and Title', Pattern: '{Content.ParentPath}{Content.Slug}', Description: 'groups/my-group/my-department/my-post'}]")
                    .WithSetting("AutorouteSettings.PerItemConfiguration", "false")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
            );

            SchemaBuilder.CreateTable("RecentDepartmentsPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("DepartmentId")
                    .Column<int>("Count")
                );

            ContentDefinitionManager.AlterPartDefinition("RecentDepartmentsPart", part => part
                .WithDescription("Renders a list of recent group posts."));

            ContentDefinitionManager.AlterTypeDefinition("RecentDepartments",
                cfg => cfg
                    .WithPart("RecentDepartmentsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 5;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("GroupPartRecord", command => command.AddColumn<int>("Weight"));

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterPartDefinition("GroupPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Group Type with a hierarchy of different departments/posts"));

            ContentDefinitionManager.AlterPartDefinition("DepartmentPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Department Type, useful when wanting different types of departments for different groups"));

            ContentDefinitionManager.AlterPartDefinition("GroupPostPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Post Type, useful when wanting different types of posts for different groups"));


            return 3;
        }

        public int UpdateFrom3()
        {

            SchemaBuilder.CreateTable("GroupPostPartRecord",
                table => table
                    .ContentPartVersionRecord()
                    .Column<int>("RepliedOn", column => column.WithDefault(null))
                    .Column<string>("Text", column => column.Unlimited())
                    .Column<string>("Format")
                );

            ContentDefinitionManager.AlterPartDefinition("GroupPostPart", builder => builder
                .Attachable()
                .WithDescription("Create your own Post Type, useful when wanting different types of posts for different groups"));

            ContentDefinitionManager.AlterTypeDefinition("GroupPost", cfg => cfg
                .WithPart("GroupPostPart")
                .WithPart("CommonPart", builder => builder
                    .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
            );

            return 4;
        }

        public int UpdateFrom4()
        {

            //SchemaBuilder.CreateTable("RecentDepartmentsPartRecord",
            //    table => table
            //        .ContentPartRecord()
            //        .Column<int>("DepartmentId")
            //        .Column<int>("Count")
            //    );

            //ContentDefinitionManager.AlterPartDefinition("RecentDepartmentsPart", part => part
            //    .WithDescription("Renders a list of recent group posts."));

            //ContentDefinitionManager.AlterTypeDefinition("RecentDepartments",
            //    cfg => cfg
            //        .WithPart("RecentDepartmentsPart")
            //        .WithPart("CommonPart")
            //        .WithPart("WidgetPart")
            //        .WithSetting("Stereotype", "Widget")
            //    );

            return 5;
        }

        public int UpdateFrom5()
        {

            SchemaBuilder.CreateTable("RecentDepartmentsPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("DepartmentName")
                    .Column<int>("Count")
                );

            ContentDefinitionManager.AlterPartDefinition("RecentDepartmentsPart", part => part
                .WithDescription("Renders a list of recent group posts."));

            ContentDefinitionManager.AlterTypeDefinition("RecentDepartments",
                cfg => cfg
                    .WithPart("RecentDepartmentsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 6;
        }

        public int UpdateFrom6()
        {

            SchemaBuilder.AlterTable("DepartmentPartRecord",
                table => table
                .AddColumn("Description", DbType.String)
                );

            return 7;
        }

    }
}