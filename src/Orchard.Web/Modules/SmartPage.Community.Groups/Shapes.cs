using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Localization;
using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups {
    public class Shapes : IShapeTableProvider {
        public Shapes() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Post_Body_Editor")
                .OnDisplaying(displaying => {
                    string flavor = displaying.Shape.EditorFlavor;
                    displaying.ShapeMetadata.Alternates.Add("Post_Body_Editor__" + flavor);
                });

            // We support multiple group types, but need to be able to skin group admin shapes, so add alternates for any content type that has a GroupPart.
            builder.Describe("Content").OnDisplaying(displaying => {
                var content = (ContentItem) displaying.Shape.ContentItem;

                if (!content.Parts.Any(x => x.PartDefinition.Name == typeof (GroupPart).Name || x.PartDefinition.Name== typeof(GroupPostPart).Name))
                    return;

                var displayType = !string.IsNullOrWhiteSpace(displaying.ShapeMetadata.DisplayType) ? displaying.ShapeMetadata.DisplayType : "Detail";
                var alternates = new[] {
                    string.Format("Content__{0}", content.ContentType),
                    string.Format("Content__{0}_{1}", content.ContentType, displayType),

                };

                foreach (var alternate in alternates.Where(alternate => !displaying.ShapeMetadata.Alternates.Contains(alternate))) {
                    displaying.ShapeMetadata.Alternates.Add(alternate);
                }
            });
        }
    }
}
