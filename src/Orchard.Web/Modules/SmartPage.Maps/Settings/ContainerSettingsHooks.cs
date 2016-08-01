using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace SmartPage.Maps.Settings {
    public class ContainerSettingsHooks : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition)
        {
            if (definition.PartDefinition.Name != "MapPart")
                yield break;

            var model = definition.Settings.GetModel<MapPartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.Name != "MapPart")
                yield break;

            var model = new MapPartSettings();
            updateModel.TryUpdateModel(model, "MapPartSettings", null, null);
            builder.WithSetting("MapPartSettings.Height", model.Height.ToString());
            builder.WithSetting("MapPartSettings.DefaultLatitude", model.DefaultLatitude.ToString("R", CultureInfo.InvariantCulture));
            builder.WithSetting("MapPartSettings.DefaultLongitude", model.DefaultLongitude.ToString("R",CultureInfo.InvariantCulture));
            builder.WithSetting("MapPartSettings.ApiKey", model.ApiKey);

            yield return DefinitionTemplate(model);
        }
    }
}