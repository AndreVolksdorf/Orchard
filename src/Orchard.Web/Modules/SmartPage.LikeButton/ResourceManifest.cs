using Orchard.UI.Resources;

namespace SmartPage.LikeButton
{
    public class ResourceManifest : IResourceManifestProvider
    {

        #region IResourceManifestProvider Member

        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineStyle("LikeButton").SetUrl("LikeButton.css");
        }

        #endregion
    }
}