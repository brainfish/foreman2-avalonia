using Foreman.DataCaching;
using System;
using System.Resources;
using System.Windows.Forms;

[assembly: NeutralResourcesLanguage("en")]

namespace Foreman {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            UpgradeUserSettingsIfNeeded();
            ErrorLogging.ClearLog();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.Run();
        }

        /// <summary>
        /// Each assembly version stores settings in a new folder; copy the previous version's values on first launch.
        /// </summary>
        private static void UpgradeUserSettingsIfNeeded() {
            if (!Properties.Settings.Default.UpgradeRequired)
                return;

            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.UpgradeRequired = false;
            Properties.Settings.Default.Save();
        }
    }
}
