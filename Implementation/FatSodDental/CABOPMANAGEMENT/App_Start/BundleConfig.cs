using System.Web.Optimization;

namespace CABOPMANAGEMENT
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-1.12.1/jquery-ui.js",
                        "~/Scripts/DataTables/datatables.min.js",
                        "~/Scripts/dataTables.buttons.min.js",
                        "~/Scripts/dataTables.select.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/jstools").Include(
                        "~/Scripts/js/buttons.flash.min.js",
                        "~/Scripts/js/jszip.min.js",
                        "~/Scripts/js/pdfmake.min.js",
                        "~/Scripts/js/vfs_fonts.js",
                        "~/Scripts/js/buttons.html5.min.js",
                        "~/Scripts/js/buttons.print.min.js"
                        ));
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Scripts/jquery-ui-1.12.1/jquery-ui.css",
                      "~/Scripts/jquery-ui-1.12.1/jquery-ui.structure.css",
                      "~/Scripts/jquery-ui-1.12.1/jquery-ui.theme.css",
                      "~/Scripts/DataTables/datatables.min.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/buttons.dataTables.min.css",
                      "~/Content/site.css"));
        }
    }
}
