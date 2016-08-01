using Orchard.UI.Resources;

namespace SmartPage.Maps {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var resourceManifest = builder.Add();

            resourceManifest.DefineScript("maps").SetUrl("jmelosegui.googlemap.js").SetDependencies("jQuery");
        }
    }
}