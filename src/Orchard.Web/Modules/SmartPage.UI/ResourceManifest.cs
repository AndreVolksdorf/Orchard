using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Resources;

namespace SmartPage.UI
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("typeahead").SetUrl("typeahead.jquery.js").SetDependencies("jQuery");

            manifest.DefineScript("svginject.js").SetUrl("svginject.min.js").SetDependencies("jQuery");

            manifest.DefineScript("bootstrap").SetUrl("bootstrap.js").SetDependencies("jQuery");

            manifest.DefineScript("bootgrid").SetUrl("jquery.bootgrid.js").SetDependencies("jQuery");
            manifest.DefineScript("bootgrid-fa").SetUrl("jquery.bootgrid.fa.js").SetDependencies("bootgrid");

            manifest.DefineScript("bootstrap-datetimepicker").SetUrl("bootstrap-datetimepicker.js").SetDependencies("bootstrap");

            manifest.DefineScript("bootstrap-select").SetUrl("bootstrap-select.js").SetDependencies("bootstrap");
            manifest.DefineScript("bootstrap-select-de").SetUrl("i18n/bootstrap-select/defaults-de_DE.js").SetDependencies("bootstrap-select");

            manifest.DefineScript("fileinput").SetUrl("fileinput.js").SetDependencies("jQuery");
            manifest.DefineScript("fileinput-purify").SetUrl("plugins/purify.js").SetDependencies("fileinput");
            manifest.DefineScript("fileinput-sortable").SetUrl("plugins/sortable.js").SetDependencies("fileinput");
            manifest.DefineScript("fileinput-canvas-to-blob").SetUrl("plugins/canvas-to-blob.js").SetDependencies("fileinput");
            manifest.DefineScript("fileinput-de").SetUrl("i18n/fileinput/de.js").SetDependencies("fileinput");

            manifest.DefineScript("equalize").SetUrl("equalize.js").SetDependencies("jQuery");
            manifest.DefineScript("moment").SetUrl("moment.js").SetDependencies("jQuery");
        }
    }
}