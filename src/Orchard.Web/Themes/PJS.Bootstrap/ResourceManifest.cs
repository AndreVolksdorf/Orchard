using Orchard.UI.Resources;

namespace PJS.Bootstrap {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("Bootstrap").SetUrl("bootstrap-3.3.5/js/bootstrap.min.js", "bootstrap-3.3.5/js/bootstrap.js").SetVersion("3.3.4").SetDependencies("jQuery");
            manifest.DefineScript("HoverDropdown").SetUrl("hover-dropdown.js").SetDependencies("Bootstrap");
            manifest.DefineScript("Stapel-Modernizr").SetUrl("stapel/modernizr.custom.63321.js");
            manifest.DefineScript("Stapel").SetUrl("stapel/jquery.stapel.js").SetDependencies("jQuery", "Stapel-Modernizr");
            manifest.DefineScript("prettyPhoto").SetUrl("prettyPhoto/jquery.prettyPhoto.js").SetDependencies("jQuery");
            manifest.DefineScript("CustomBase").SetUrl("custom.js").SetDependencies("jQuery");
            manifest.DefineScript("Equalizer").SetUrl("equalize.min.js");
            manifest.DefineScript("Moment").SetUrl("moment.min.js");
            manifest.DefineScript("BootstrapMultiselect").SetUrl("bootstrap-multiselect.js").SetDependencies("Bootstrap");
            manifest.DefineScript("BootstrapMultiselectTaxonomy").SetUrl("bootstrap-multiselect-taxonomy.js").SetDependencies("Bootstrap");
            manifest.DefineScript("MomentLocalized").SetUrl("moment-with-locales.min.js");
            manifest.DefineScript("BootstrapDatetimepicker").SetUrl("bootstrap-datetimepicker.js").SetDependencies("Bootstrap", "MomentLocalized");

            manifest.DefineStyle("Stapel").SetUrl("stapel/stapel.css");
            manifest.DefineStyle("prettyPhoto").SetUrl("prettyPhoto/prettyPhoto.css");
        }
    }
}
