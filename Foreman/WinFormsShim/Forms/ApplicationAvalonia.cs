using System.ComponentModel;
using System.Drawing;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using AvApp = Avalonia.Application;
using AvKey = Avalonia.Input.Key;

namespace System.Windows.Forms {
    public static class Application {
        public static string StartupPath => AppContext.BaseDirectory;
        public static string ExecutablePath => Environment.ProcessPath ?? StartupPath;
        public static string ProductName => "Foreman";
        public static string ProductVersion => "2.0";
        public static string CompanyName => "";
        public static string UserAppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Foreman");
        public static bool UseWaitCursor { get; set; }
        public static Form? MainForm { get; internal set; }
        public static FormCollection OpenForms { get; } = [];
        public static void EnableVisualStyles() { }
        public static void SetCompatibleTextRenderingDefault(bool _) { }
        public static void SetHighDpiMode(HighDpiMode _) { }
        public static void DoEvents() { }
        public static void Exit() {
            if (AvApp.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life)
                life.Shutdown();
        }
        public static void ExitThread() => Exit();
        public static void Run() {
            AvaloniaBootstrap.Run();
        }
        public static void Run(Form form) {
            MainForm = form;
            OpenForms.Add(form);
            AvaloniaBootstrap.PendingMainForm = form;
            AvaloniaBootstrap.Run();
        }
        public static void Restart() { }
    }
    public class FormCollection : List<Form> { }

    public static class ControlPaint {
        public static void DrawBorder3D(Graphics g, Rectangle r) => g.DrawRectangle(Pens.Gray, r);
        public static void DrawFocusRectangle(Graphics g, Rectangle r) => g.DrawRectangle(Pens.Black, r);
    }
}

namespace System.Windows.Forms {
    internal static class AvaloniaBootstrap {
        public static MouseButtons CurrentMouseButtons { get; set; }
        public static Drawing.Point ScreenMousePosition { get; set; }
        private static readonly HashSet<AvKey> DownKeys = [];
        private static bool _initialized;
        public static bool IsKeyDown(AvKey key) => DownKeys.Contains(key);
        public static void NoteKey(AvKey key, bool down) {
            if (down) DownKeys.Add(key); else DownKeys.Remove(key);
        }

        public static void EnsureInitialized() {
            if (_initialized || AvApp.Current is not null) { _initialized = true; return; }
            _initialized = true;
            AppBuilder.Configure<ForemanAvaloniaApp>()
                .UsePlatformDetect()
                .WithInterFont()
                .SetupWithoutStarting();
        }

        public static void Run() {
            AppBuilder.Configure<ForemanAvaloniaApp>()
                .UsePlatformDetect()
                .WithInterFont()
                .StartWithClassicDesktopLifetime(Environment.GetCommandLineArgs(), ShutdownMode.OnMainWindowClose);
        }

        internal static Form? PendingMainForm { get; set; }
    }

    internal sealed class ForemanAvaloniaApp : AvApp {
        public override void Initialize() {
            Styles.Add(new FluentTheme());
            RequestedThemeVariant = ThemeVariant.Light;
        }
        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                Form form = AvaloniaBootstrap.PendingMainForm ?? new Foreman.MainForm();
                Application.MainForm = form;
                if (!Application.OpenForms.Contains(form))
                    Application.OpenForms.Add(form);
                AvaloniaBootstrap.PendingMainForm = form;
                desktop.MainWindow = form.Window;
                form.Show();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}

namespace System.ComponentModel {
    public class ComponentResourceManager {
        public ComponentResourceManager(Type _) { }
        public object? GetObject(string _) => null;
        public string? GetString(string _) => null;
        public void ApplyResources(object _, string __) { }
    }
}

namespace Microsoft.Win32 {
    public static class Registry {
        public static object? GetValue(string _, string __, object? defaultValue) => defaultValue;
    }
}
