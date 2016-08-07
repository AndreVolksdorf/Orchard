using Orchard.UI.Resources;

namespace SmartPage.Community.Groups {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var resourceManifest = builder.Add();
            resourceManifest.DefineStyle("GroupsAdmin").SetUrl("ngm-group-admin.css");

            resourceManifest.DefineStyle("DepartmentsAdmin").SetUrl("ngm-department-admin.css");
        }
    }
}