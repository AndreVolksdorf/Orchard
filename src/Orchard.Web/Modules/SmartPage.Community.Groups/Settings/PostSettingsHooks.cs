using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Common.Settings;

namespace SmartPage.Community.Groups.Settings {
    public class PostSettingsHooks : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "GroupPostPart")
                yield break;

            var model = definition.Settings.GetModel<GroupPostTypePartSettings>();

            if (string.IsNullOrWhiteSpace(model.Flavor)) {
                var partModel = definition.PartDefinition.Settings.GetModel<GroupPostPartSettings>();
                model.Flavor = partModel.FlavorDefault;
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartEditor(ContentPartDefinition definition) {
            if (definition.Name != "GroupPostPart")
                yield break;

            var model = definition.Settings.GetModel<GroupPostPartSettings>();
            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "GroupPostPart")
                yield break;

            var model = new BodyTypePartSettings();
            updateModel.TryUpdateModel(model, "GroupPostTypePartSettings", null, null);
            builder.WithSetting("GroupPostTypePartSettings.Flavor", !string.IsNullOrWhiteSpace(model.Flavor) ? model.Flavor : null);
            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartEditorUpdate(ContentPartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "GroupPostPart")
                yield break;

            var model = new BodyPartSettings();
            updateModel.TryUpdateModel(model, "GroupPostPartSettings", null, null);
            builder.WithSetting("GroupPostPartSettings.FlavorDefault", !string.IsNullOrWhiteSpace(model.FlavorDefault) ? model.FlavorDefault : null);
            yield return DefinitionTemplate(model);
        }
    }
}