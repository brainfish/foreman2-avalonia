using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvWindow = Avalonia.Controls.Window;
using AvControl = Avalonia.Controls.Control;
using AvMenu = Avalonia.Controls.ContextMenu;
using AvMenuItem = Avalonia.Controls.MenuItem;
using Image = System.Drawing.Image;

namespace System.Windows.Forms {
    public partial class Form : ContainerControl {
        internal AvWindow Window { get; }
        public FormBorderStyle FormBorderStyle { get; set; } = FormBorderStyle.Sizable;
        public FormStartPosition StartPosition { get; set; } = FormStartPosition.WindowsDefaultLocation;
        public FormWindowState WindowState { get; set; }
        public bool ShowInTaskbar { get; set; } = true;
        public bool ShowIcon { get; set; } = true;
        public bool ControlBox { get; set; } = true;
        public bool MinimizeBox { get; set; } = true;
        public bool MaximizeBox { get; set; } = true;
        public bool HelpButton { get; set; }
        public bool KeyPreview { get; set; }
        public bool TopMost { get; set; }
        public IWin32Window? Owner { get; set; }
        public Icon? Icon { get; set; }
        public DialogResult DialogResult { get; set; }
        public IButtonControl? AcceptButton { get; set; }
        public IButtonControl? CancelButton { get; set; }
        public bool Modal { get; private set; }
        public bool IsMdiContainer { get; set; }
        public MainMenu? Menu { get; set; }
        public event FormClosingEventHandler? FormClosing;
        public event FormClosedEventHandler? FormClosed;
        public event EventHandler? Shown;

        public Form() {
            Window = new AvWindow {
                Content = Native,
                Width = 800,
                Height = 600,
            };
            Window.Closing += (_, e) => {
                var args = new FormClosingEventArgs(CloseReason.UserClosing, false);
                OnFormClosing(args);
                e.Cancel = args.Cancel;
            };
            Window.Closed += (_, _) => OnFormClosed(new FormClosedEventArgs(CloseReason.UserClosing));
            Window.Opened += (_, _) => { RaiseLoad(); Shown?.Invoke(this, EventArgs.Empty); };
            Native.Width = double.NaN;
            Native.Height = double.NaN;
            Size = new Size(800, 600);
        }

        public override string Text { get => Window.Title ?? ""; set { Window.Title = value; base.Text = value; } }
        public override Size ClientSize {
            get => new((int)(Window.ClientSize.Width > 0 ? Window.ClientSize.Width : Width), (int)(Window.ClientSize.Height > 0 ? Window.ClientSize.Height : Height));
            set { Window.Width = value.Width; Window.Height = value.Height + 40; Size = new Size(value.Width, value.Height + 40); }
        }

        public void Close() {
            var args = new FormClosingEventArgs(CloseReason.UserClosing, false);
            OnFormClosing(args);
            if (args.Cancel) return;
            Window.Close();
        }
        public new void Show() {
            ApplyWindowChrome();
            Window.Show();
        }
        public DialogResult ShowDialog() => ShowDialog(null);
        public DialogResult ShowDialog(IWin32Window? owner) {
            Modal = true;
            ApplyWindowChrome();
            DialogResult = DialogResult.None;
            AvWindow? parent = (owner as Form)?.Window ?? Application.MainForm?.Window;
            DialogPump.ShowModal(Window, parent);
            Modal = false;
            return DialogResult == DialogResult.None ? DialogResult.Cancel : DialogResult;
        }
        protected virtual void OnFormClosing(FormClosingEventArgs e) => FormClosing?.Invoke(this, e);
        protected virtual void OnFormClosed(FormClosedEventArgs e) => FormClosed?.Invoke(this, e);
        private void ApplyWindowChrome() {
            Window.WindowState = WindowState switch {
                FormWindowState.Maximized => Avalonia.Controls.WindowState.Maximized,
                FormWindowState.Minimized => Avalonia.Controls.WindowState.Minimized,
                _ => Avalonia.Controls.WindowState.Normal
            };
            Window.CanResize = FormBorderStyle is FormBorderStyle.Sizable or FormBorderStyle.SizableToolWindow;
            Window.SystemDecorations = FormBorderStyle == FormBorderStyle.None ? SystemDecorations.None : SystemDecorations.Full;
            Window.ShowInTaskbar = ShowInTaskbar;
            Window.Topmost = TopMost;
            if (StartPosition == FormStartPosition.CenterScreen)
                Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else if (StartPosition == FormStartPosition.CenterParent)
                Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
    public interface IButtonControl {
        DialogResult DialogResult { get; set; }
        void NotifyDefault(bool value);
        void PerformClick();
    }
    public class MainMenu { }

    public class ToolStripItem : Component {
        public string Text { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public bool Checked { get; set; }
        public bool CheckOnClick { get; set; }
        public object? Tag { get; set; }
        public System.Drawing.Image? Image { get; set; }
        public event EventHandler? Click;
        public ToolStripItemCollection DropDownItems { get; } = [];
        internal void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
        public override string ToString() => Text;
    }
    public class ToolStripMenuItem : ToolStripItem {
        public ToolStripMenuItem() { }
        public ToolStripMenuItem(string text) { Text = text; }
        public ToolStripMenuItem(string text, Image? image, EventHandler? onClick) {
            Text = text; Image = image;
            if (onClick is not null) Click += onClick;
        }
        public Keys ShortcutKeys { get; set; }
        public bool ShowShortcutKeys { get; set; } = true;
        public Size Size { get; set; }
    }
    public class ToolStripSeparator : ToolStripItem { }
    public class ToolStripItemCollection : IEnumerable<ToolStripItem> {
        private readonly List<ToolStripItem> _i = [];
        public int Count => _i.Count;
        public ToolStripItem this[int i] => _i[i];
        public ToolStripItem Add(string text) { var it = new ToolStripMenuItem(text); _i.Add(it); return it; }
        public int Add(ToolStripItem item) { _i.Add(item); return _i.Count - 1; }
        public void AddRange(ToolStripItem[] items) => _i.AddRange(items);
        public void Clear() => _i.Clear();
        public IEnumerator<ToolStripItem> GetEnumerator() => _i.GetEnumerator();
        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class ToolStripDropDown : Control {
        public ToolStripItemCollection Items { get; } = [];
        public bool ShowItemToolTips { get; set; }
        public bool ShowImageMargin { get; set; } = true;
        public bool ShowCheckMargin { get; set; }
        public event ToolStripDropDownClosingEventHandler? Closing;
        public event EventHandler? Closed;
        public event ToolStripItemClickedEventHandler? ItemClicked;
        public void Show() => Show(null, Point.Empty);
        public void Show(Point screenLocation) => Show(null, screenLocation);
        public void Show(Control? _, Point __) { }
        public void Close() => Hide();
        public Size ImageScalingSize { get; set; } = new(16, 16);
        internal void RaiseItemClicked(ToolStripItem item) => ItemClicked?.Invoke(this, new ToolStripItemClickedEventArgs(item));
        internal void RaiseClosing(ToolStripDropDownClosingEventArgs e) => Closing?.Invoke(this, e);
        internal void RaiseClosed() => Closed?.Invoke(this, EventArgs.Empty);
    }
    public class ContextMenuStrip : ToolStripDropDown {
        public ContextMenuStrip() { }
        public ContextMenuStrip(IContainer _) { }
        public void Show(Control control, Point pos) {
            var menu = new AvMenu();
            foreach (ToolStripItem item in Items) {
                if (item is ToolStripSeparator) { menu.Items.Add(new Avalonia.Controls.Separator()); continue; }
                var mi = new AvMenuItem { Header = item.Text, IsEnabled = item.Enabled };
                var captured = item;
                mi.Click += (_, _) => {
                    captured.RaiseClick();
                    RaiseItemClicked(captured);
                    var ce = new ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason.ItemClicked);
                    RaiseClosing(ce);
                    if (!ce.Cancel) RaiseClosed();
                };
                menu.Items.Add(mi);
            }
            if (control.Native is Avalonia.Controls.Control native)
                menu.Open(native);
        }
    }

    public class ToolTip : Component {
        public int AutoPopDelay { get; set; } = 5000;
        public int InitialDelay { get; set; } = 500;
        public int ReshowDelay { get; set; } = 100;
        public bool OwnerDraw { get; set; }
        public bool ShowAlways { get; set; }
        public Color BackColor { get; set; } = Color.FromArgb(255, 255, 255, 225);
        public Color ForeColor { get; set; } = Color.Black;
        public event PopupEventHandler? Popup;
        public event DrawToolTipEventHandler? Draw;
        private readonly Dictionary<Control, string> _tips = [];
        public ToolTip() { }
        public ToolTip(IContainer _) { }
        public void SetToolTip(Control c, string caption) => _tips[c] = caption;
        public string GetToolTip(Control c) => _tips.TryGetValue(c, out var t) ? t : "";
        public void Show(string text, IWin32Window window) => Show(text, window, Point.Empty);
        public void Show(string text, IWin32Window window, Point location) {
            var args = new PopupEventArgs();
            Popup?.Invoke(this, args);
            if (args.Cancel) return;
            if (OwnerDraw && window is Control c) {
                using var bmp = new Bitmap(Math.Max(1, args.ToolTipSize.Width), Math.Max(1, args.ToolTipSize.Height));
                using var g = Graphics.FromImage(bmp);
                Draw?.Invoke(this, new DrawToolTipEventArgs(g, c, c, new Rectangle(Point.Empty, args.ToolTipSize), text, BackColor, ForeColor, SystemFonts.DefaultFont));
            }
        }
        public void Hide(IWin32Window _) { }
        public void RemoveAll() => _tips.Clear();
    }

    public class Timer : Component {
        private readonly DispatcherTimer _t = new();
        public int Interval { get => (int)_t.Interval.TotalMilliseconds; set => _t.Interval = TimeSpan.FromMilliseconds(Math.Max(1, value)); }
        public bool Enabled { get => _t.IsEnabled; set => _t.IsEnabled = value; }
        public event EventHandler? Tick;
        public Timer() { _t.Tick += (_, _) => Tick?.Invoke(this, EventArgs.Empty); }
        public Timer(IContainer _) : this() { }
        public void Start() => Enabled = true;
        public void Stop() => Enabled = false;
        protected override void Dispose(bool disposing) { if (disposing) _t.Stop(); base.Dispose(disposing); }
    }

    public class Cursor {
        internal Avalonia.Input.Cursor Native { get; }
        public Cursor(Avalonia.Input.StandardCursorType t) { Native = new Avalonia.Input.Cursor(t); }
        public static Point Position {
            get => AvaloniaBootstrap.ScreenMousePosition;
            set => AvaloniaBootstrap.ScreenMousePosition = value;
        }
        public static implicit operator Cursor(IntPtr _) => Cursors.Default;
    }
    public static class Cursors {
        public static Cursor Default { get; } = new(Avalonia.Input.StandardCursorType.Arrow);
        public static Cursor Arrow { get; } = Default;
        public static Cursor Cross { get; } = new(Avalonia.Input.StandardCursorType.Cross);
        public static Cursor Hand { get; } = new(Avalonia.Input.StandardCursorType.Hand);
        public static Cursor IBeam { get; } = new(Avalonia.Input.StandardCursorType.Ibeam);
        public static Cursor SizeAll { get; } = new(Avalonia.Input.StandardCursorType.SizeAll);
        public static Cursor SizeNS { get; } = new(Avalonia.Input.StandardCursorType.SizeNorthSouth);
        public static Cursor SizeWE { get; } = new(Avalonia.Input.StandardCursorType.SizeWestEast);
        public static Cursor SizeNWSE { get; } = new(Avalonia.Input.StandardCursorType.TopLeftCorner);
        public static Cursor SizeNESW { get; } = new(Avalonia.Input.StandardCursorType.TopRightCorner);
        public static Cursor WaitCursor { get; } = new(Avalonia.Input.StandardCursorType.Wait);
        public static Cursor No { get; } = new(Avalonia.Input.StandardCursorType.No);
        public static Cursor AppStarting { get; } = WaitCursor;
        public static Cursor Help { get; } = Default;
        public static Cursor HSplit { get; } = SizeNS;
        public static Cursor VSplit { get; } = SizeWE;
    }

    public static class Clipboard {
        private static string _text = "";
        public static void SetText(string text) {
            _text = text ?? "";
            try {
                var top = Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d
                    ? d.MainWindow : null;
                top?.Clipboard?.SetTextAsync(_text);
            } catch { /* headless / tests */ }
        }
        public static string GetText() {
            try {
                var top = Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d
                    ? d.MainWindow : null;
                string? t = top?.Clipboard?.GetTextAsync().GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(t)) return t;
            } catch { }
            return _text;
        }
        public static bool ContainsText() => !string.IsNullOrEmpty(GetText());
        public static void Clear() => SetText("");
        public static void SetDataObject(object data) { if (data is string s) SetText(s); }
        public static IDataObject GetDataObject() => new DataObject(GetText());
    }

    public static class MessageBox {
        public static DialogResult Show(string text) => Show(null, text, "", MessageBoxButtons.OK, MessageBoxIcon.None);
        public static DialogResult Show(string text, string caption) => Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) => Show(null, text, caption, buttons, MessageBoxIcon.None);
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => Show(null, text, caption, buttons, icon);
        public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
            if (UserMessageHook.Handler is { } h) return h(text, caption, buttons, icon);
            var window = new AvWindow {
                Title = caption ?? "Foreman",
                Width = 420, Height = 180, CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = BuildMessage(text, buttons, out var resultHolder)
            };
            AvWindow? parent = (owner as Form)?.Window ?? Application.MainForm?.Window;
            DialogPump.ShowModal(window, parent);
            return resultHolder.Value;
        }
        private static AvControl BuildMessage(string text, MessageBoxButtons buttons, out Holder result) {
            result = new Holder { Value = DialogResult.OK };
            var panel = new Avalonia.Controls.StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 12 };
            panel.Children.Add(new Avalonia.Controls.TextBlock { Text = text, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
            var row = new Avalonia.Controls.StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
            void Add(string caption, DialogResult dr, Holder hold) {
                var b = new Avalonia.Controls.Button { Content = caption, MinWidth = 80 };
                b.Click += (_, _) => {
                    hold.Value = dr;
                    (TopLevel.GetTopLevel(b) as AvWindow)?.Close();
                };
                row.Children.Add(b);
            }
            var hold = result;
            switch (buttons) {
                case MessageBoxButtons.OKCancel: Add("OK", DialogResult.OK, hold); Add("Cancel", DialogResult.Cancel, hold); break;
                case MessageBoxButtons.YesNo: Add("Yes", DialogResult.Yes, hold); Add("No", DialogResult.No, hold); break;
                case MessageBoxButtons.YesNoCancel: Add("Yes", DialogResult.Yes, hold); Add("No", DialogResult.No, hold); Add("Cancel", DialogResult.Cancel, hold); break;
                default: Add("OK", DialogResult.OK, hold); break;
            }
            panel.Children.Add(row);
            return panel;
        }
        private sealed class Holder { public DialogResult Value; }
    }
    internal static class UserMessageHook {
        public static Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult>? Handler;
    }

    public abstract class FileDialog : CommonDialog {
        public string Filter { get; set; } = "";
        public int FilterIndex { get; set; } = 1;
        public string FileName { get; set; } = "";
        public string InitialDirectory { get; set; } = "";
        public string Title { get; set; } = "";
        public bool AddExtension { get; set; } = true;
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; } = true;
        public bool RestoreDirectory { get; set; }
        public bool ValidateNames { get; set; } = true;
        public bool DereferenceLinks { get; set; } = true;
        public string DefaultExt { get; set; } = "";
        public string[] FileNames { get; protected set; } = [];
        public bool Multiselect { get; set; }
    }
    public abstract class CommonDialog : Component {
        public event EventHandler? HelpRequest;
        public DialogResult ShowDialog() => ShowDialog(null);
        public DialogResult ShowDialog(IWin32Window? owner) => RunDialog(owner);
        protected abstract DialogResult RunDialog(IWin32Window? owner);
        internal static Avalonia.Platform.Storage.IStorageProvider? StorageProvider(IWin32Window? owner) {
            AvWindow? w = (owner as Form)?.Window ?? Application.MainForm?.Window;
            return w?.StorageProvider;
        }
    }
    public class OpenFileDialog : FileDialog {
        public bool ShowReadOnly { get; set; }
        public bool ReadOnlyChecked { get; set; }
        protected override DialogResult RunDialog(IWin32Window? owner) {
            var sp = StorageProvider(owner);
            if (sp is null) return DialogResult.Cancel;
            var files = DialogPump.Await(sp.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions {
                AllowMultiple = Multiselect,
                Title = Title,
            }));
            if (files is null || files.Count == 0) return DialogResult.Cancel;
            FileName = files[0].Path.LocalPath;
            FileNames = files.Select(f => f.Path.LocalPath).ToArray();
            return DialogResult.OK;
        }
    }
    public class SaveFileDialog : FileDialog {
        public bool OverwritePrompt { get; set; } = true;
        public bool CreatePrompt { get; set; }
        protected override DialogResult RunDialog(IWin32Window? owner) {
            var sp = StorageProvider(owner);
            if (sp is null) return DialogResult.Cancel;
            var file = DialogPump.Await(sp.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions {
                Title = Title,
                SuggestedFileName = FileName,
                DefaultExtension = DefaultExt,
            }));
            if (file is null) return DialogResult.Cancel;
            FileName = file.Path.LocalPath;
            return DialogResult.OK;
        }
    }
    public class FolderBrowserDialog : CommonDialog {
        public string Description { get; set; } = "";
        public string SelectedPath { get; set; } = "";
        public Environment.SpecialFolder RootFolder { get; set; }
        public bool ShowNewFolderButton { get; set; } = true;
        public bool UseDescriptionForTitle { get; set; }
        protected override DialogResult RunDialog(IWin32Window? owner) {
            var sp = StorageProvider(owner);
            if (sp is null) return DialogResult.Cancel;
            var folders = DialogPump.Await(sp.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions { Title = Description, AllowMultiple = false }));
            if (folders is null || folders.Count == 0) return DialogResult.Cancel;
            SelectedPath = folders[0].Path.LocalPath;
            return DialogResult.OK;
        }
    }
    public class ColorDialog : CommonDialog {
        public Color Color { get; set; } = Color.Black;
        public bool FullOpen { get; set; }
        public bool AnyColor { get; set; }
        public bool SolidColorOnly { get; set; }
        public int[] CustomColors { get; set; } = [];
        protected override DialogResult RunDialog(IWin32Window? owner) => DialogResult.OK;
    }
    internal static class DialogPump {
        public static void ShowModal(AvWindow window, AvWindow? owner) {
            using var cts = new CancellationTokenSource();
            window.Closed += (_, _) => cts.Cancel();
            if (owner is not null)
                _ = window.ShowDialog(owner);
            else
                window.Show();
            try {
                Dispatcher.UIThread.MainLoop(cts.Token);
            } catch (OperationCanceledException) { }
        }
        public static T Await<T>(Task<T> task) {
            if (task.IsCompleted) return task.GetAwaiter().GetResult();
            using var cts = new CancellationTokenSource();
            task.ContinueWith(_ => cts.Cancel(), TaskScheduler.Default);
            try {
                if (Dispatcher.UIThread.CheckAccess())
                    Dispatcher.UIThread.MainLoop(cts.Token);
                else
                    task.Wait();
            } catch (OperationCanceledException) { }
            return task.GetAwaiter().GetResult();
        }
    }

    public class FontDialog : CommonDialog {
        public Font? Font { get; set; }
        public Color Color { get; set; }
        public bool ShowColor { get; set; }
        public bool ShowEffects { get; set; } = true;
        public bool FontMustExist { get; set; }
        protected override DialogResult RunDialog(IWin32Window? owner) => DialogResult.OK;
    }
}
