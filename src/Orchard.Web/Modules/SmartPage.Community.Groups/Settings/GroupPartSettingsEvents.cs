using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Localization;
using SmartPage.Community.Groups.Extensions;

namespace SmartPage.Community.Groups.Settings {
    public class GroupPartSettingsEvents : ContentDefinitionEditorEventsBase {

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "GroupPart")
                yield break;

            var settings = definition.Settings.GetModel<GroupPartSettings>();

            if (string.IsNullOrWhiteSpace(settings.PostType)) {
                settings.PostType = Constants.Parts.Post;
            }

            if (string.IsNullOrWhiteSpace(settings.DepartmentType)) {
                settings.DepartmentType = Constants.Parts.Department;
            }

            yield return DefinitionTemplate(settings);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "GroupPart")
                yield break;

            var settings = new GroupPartSettings();

            if (updateModel.TryUpdateModel(settings, "GroupPartSettings", null, null)) {
                builder.WithSetting("GroupPartSettings.DefaultDepartmentedPosts", settings.DefaultDepartmentedPosts.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("GroupPartSettings.PostType", settings.PostType);
                builder.WithSetting("GroupPartSettings.DepartmentType", settings.DepartmentType);
            }

            yield return DefinitionTemplate(settings);
        }
    }
}