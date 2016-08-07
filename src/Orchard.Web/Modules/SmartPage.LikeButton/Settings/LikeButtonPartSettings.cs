using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartPage.LikeButton.Settings
{
    public class LikeButtonPartSettings
    {
        private bool? _showVoter;
        public bool ShowVoter
        {
            get
            {
                if (_showVoter == null)
                    _showVoter = true;
                return (bool)_showVoter;
            }
            set { _showVoter = value; }
        }

        private string _likeButtonText;

        [Required]
        [StringLength(50)]
        public string LikeButtonText
        {
            get { return _likeButtonText; }
            set { _likeButtonText = value; }
        }

        private string _unlikeButtonText;

        [Required]
        [StringLength(50)]
        public string UnlikeButtonText
        {
            get { return _unlikeButtonText; }
            set { _unlikeButtonText = value; }
        }

        private string _dimension;

        [Required]
        [StringLength(20)]
        public string Dimension
        {
            get { return _dimension; }
            set { _dimension = value; }
        }
    }

    public class ContainerSettingsHooks : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition)
        {
            if (definition.PartDefinition.Name != "LikeButtonPart")
                yield break;

            var model = definition.Settings.GetModel<LikeButtonPartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.Name != "LikeButtonPart")
                yield break;

            var model = new LikeButtonPartSettings();
            updateModel.TryUpdateModel(model, "LikeButtonPartSettings", null, null);
            builder.WithSetting("LikeButtonPartSettings.ShowVoter", model.ShowVoter.ToString());
            builder.WithSetting("LikeButtonPartSettings.UnlikeButtonText", model.UnlikeButtonText);
            builder.WithSetting("LikeButtonPartSettings.LikeButtonText", model.LikeButtonText);
            builder.WithSetting("LikeButtonPartSettings.Dimension", model.Dimension);

            yield return DefinitionTemplate(model);
        }
    }
}