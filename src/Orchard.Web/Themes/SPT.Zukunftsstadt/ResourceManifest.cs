using Orchard.UI.Resources;

namespace SPT.Zukunftsstadt {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            
            manifest.DefineScript("Custom").SetUrl("custom.js").SetDependencies("jQuery");
            manifest.DefineScript("svginject.js").SetUrl("svginject.min.js").SetDependencies("jQuery");
            manifest.DefineScript("Equalizer").SetUrl("equalize.min.js").SetDependencies("jQuery");
            
        }
    }
}
