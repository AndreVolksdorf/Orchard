using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using System;
using System.Data;

namespace Contrib.CookieCuttr
{
    public class CookiecuttrMigrations : DataMigrationImpl
    {
        public const string cookiemsg = "Wir verwenden Cookies, um Inhalte zu personalisieren, Funktionen für soziale Medien anbieten zu können und die Zugriffe auf unsere Website zu analysieren. Mehr zu Cookies <a href=\"{{cookiePolicyLink}}\" title=\"Mehr über unsere Cookies\">erfahren Sie hier</a>. Um die Webseite wie vorgesehen zu verwenden...";
        public const string cookieanalyticsmsg = "Wir verwenden Cookies, um die Zugriffe auf unsere Website zu analysieren Wir speichern keine persönlichen Daten. Um die Webseite wie vorgesehen zu verwenden...";
        public const string acceptmsg = "Akzeptieren";
        public const string declinemsg = "Ablehnen";
        public const string resetmsg = "Cookies zurücksetzen";
        public const string whataremsg = "Was sind Cookies?";
        public const string discreetmsg = "Cookies?";
        public const string errormsg = "Es tut uns leid. Dieses Feature benötigt Cookies um zu funktionieren. Sie haben die Verwendung von Cookies abgelehnt, daher wurde es deaktiviert";
        public const string whatarecookieslink = "http://www.allaboutcookies.org/";
        public int Create()
        {
            SchemaBuilder.CreateTable("CookieCuttrSettingsPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<bool>("cookieNotificationLocationBottom", c => c.WithDefault(false))
                    .Column<bool>("cookieAnalytics", c => c.WithDefault(true))
                    .Column<string>("cookieAnalyticsMessage", c => c.WithDefault(cookieanalyticsmsg).WithLength(2048))
                    .Column<string>("cookiePolicyLink", c => c.WithDefault(string.Empty))
                    .Column<bool>("showCookieDeclineButton", c => c.WithDefault(false))
                    .Column<bool>("showCookieAcceptButton", c => c.WithDefault(true))
                    .Column<bool>("showCookieResetButton", c => c.WithDefault(false))
                    .Column<bool>("cookieOverlayEnabled", c => c.WithDefault(false))
                    .Column<string>("cookieMessage", c => c.WithDefault(cookiemsg).WithLength(2048))
                    .Column<string>("cookieWhatAreTheyLink", c => c.WithDefault(whatarecookieslink))
                    .Column<bool>("cookieCutter", c => c.WithDefault(false))
                    .Column<string>("cookieErrorMessage", c => c.WithDefault(errormsg).WithLength(2048))
                    .Column<string>("cookieDisable", c => c.WithDefault(string.Empty))
                    .Column<string>("cookieAcceptButtonText", c => c.WithDefault(acceptmsg))
                    .Column<string>("cookieDeclineButtonText", c => c.WithDefault(declinemsg))
                    .Column<string>("cookieResetButtonText", c => c.WithDefault(resetmsg))
                    .Column<string>("cookieWhatAreLinkText", c => c.WithDefault(whataremsg))
                    .Column<bool>("cookiePolicyPage", c => c.WithDefault(false))
                    .Column<string>("cookiePolicyPageMessage", c => c.WithDefault(string.Empty).WithLength(2048))
                    .Column<bool>("cookieDiscreetLink", c => c.WithDefault(false))
                    .Column<bool>("cookieDiscreetReset", c => c.WithDefault(false))
                    .Column<string>("cookieDiscreetLinkText", c => c.WithDefault(discreetmsg))
                    .Column<string>("cookieDiscreetPosition", c => c.WithDefault("topleft"))
                    .Column<string>("cookieDomain", c => c.WithDefault(string.Empty))
                );

            ContentDefinitionManager.AlterPartDefinition("CookiecuttrPart", part => part
                .WithDescription("Renders the CookieCuttr plugin."));

            ContentDefinitionManager.AlterTypeDefinition("CookiecuttrWidget",
                cfg => cfg
                    .WithPart("WidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("CookiecuttrPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("CookieCuttrSettingsPartRecord", t =>
            {
                t.AlterColumn("cookieAnalyticsMessage", c => c.WithLength(2048).WithType(DbType.String));
                t.AlterColumn("cookieMessage", c => c.WithLength(2048).WithType(DbType.String));
                t.AlterColumn("cookieErrorMessage", c => c.WithLength(2048).WithType(DbType.String));
                t.AlterColumn("cookiePolicyPageMessage", c => c.WithLength(2048).WithType(DbType.String));
            });
            return 2;
        }
    }
}