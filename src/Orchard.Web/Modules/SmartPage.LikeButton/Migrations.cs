using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;
using SmartPage.LikeButton.Models;

namespace SmartPage.LikeButton
{
    public class Migrations : DataMigrationImpl
    {
        public Migrations()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(LikeButtonPart).Name, part => part
                .WithDescription(T("Renders a like button").Text)
                .Attachable()
                );

            return 1;
        }
    }
}