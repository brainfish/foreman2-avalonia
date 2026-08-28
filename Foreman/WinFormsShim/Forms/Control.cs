using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Image = System.Drawing.Image;
using AvControl = Avalonia.Controls.Control;
using AvPanel = Avalonia.Controls.Panel;
using AvPoint = Avalonia.Point;
using AvSize = Avalonia.Size;
using AvKey = Avalonia.Input.Key;
using AvVisual = Avalonia.Visual;

namespace System.Windows.Forms {
    public partial class Control : Component, IWin32Window, IDropTarget {
        internal AvControl Native { get; }
        internal WfHostPanel? HostPanel =>
            Native as WfHostPanel
            ?? (Native as Avalonia.Controls.ScrollViewer)?.Content as WfHostPanel;
        private Control? _parent;
        private string _text = "";
        private bool _visible = true;
        private bool _enabled = true;
        private Color _backColor = Color.FromArgb(255, 240, 240, 240);
        private Color _foreColor = Color.Black;
        private Font _font = SystemFonts.DefaultFont;
        private Point _location;
        private Size _size = new(100, 23);
        private Padding _margin = new(3);
        private Padding _padding;
        private DockStyle _dock;
        private AnchorStyles _anchor = AnchorStyles.Top | AnchorStyles.Left;
        private bool _autoSize;
        private int _layoutSuspend;
        private Cursor? _cursor;
        private Image? _backgroundImage;
        private static int _nextHandle = 1;
        private readonly IntPtr _handle = new(Interlocked.Increment(ref _nextHandle));

        public ControlCollection Controls { get; }
        public string Name { get; set; } = "";
        public object? Tag { get; set; }
        public int TabIndex { get; set; }
        public bool TabStop { get; set; } = true;
        public bool CausesValidation { get; set; } = true;
        public bool AllowDrop { get; set; }
        public ImeMode ImeMode { get; set; }
        public RightToLeft RightToLeft { get; set; }
        public DockStyle Dock { get => _dock; set { _dock = value; PerformParentLayout(); } }
        public AnchorStyles Anchor { get => _anchor; set => _anchor = value; }
        public Padding Margin { get => _margin; set { _margin = value; PerformParentLayout(); } }
        public Padding Padding { get => _padding; set { _padding = value; PerformLayout(); } }
        public BorderStyle BorderStyle { get; set; }
        public bool AutoSize { get => _autoSize; set { _autoSize = value; PerformLayout(); } }
        public AutoSizeMode AutoSizeMode { get; set; } = AutoSizeMode.GrowOnly;
        public Size MaximumSize { get; set; }
        public Size MinimumSize { get; set; }
        public SizeF AutoScaleDimensions { get; set; }
        public AutoScaleMode AutoScaleMode { get; set; }
        public bool DoubleBuffered { get; set; }
        public bool UseVisualStyleBackColor { get; set; }
        public ImageLayout BackgroundImageLayout { get; set; }
        public bool Capture { get; set; }
        public AccessibleObject? AccessibilityObject => null;
        public static Color DefaultBackColor => Color.FromArgb(255, 240, 240, 240);
        public static Color DefaultForeColor => Color.Black;
        public IntPtr Handle { get { CreateHandle(); return _handle; } }
        public bool IsHandleCreated { get; private set; } = true;
        public bool IsDisposed { get; private set; }
        public bool InvokeRequired => !Dispatcher.UIThread.CheckAccess();
        public virtual Rectangle Bounds {
            get => new(Location, Size);
            set { Location = value.Location; Size = value.Size; }
        }
        public Rectangle ClientRectangle => new(0, 0, ClientSize.Width, ClientSize.Height);
        public virtual Size ClientSize {
            get {
                int chrome = BorderStyle == BorderStyle.None ? 0 : 2;
                int vs = AutoScroll && VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
                int hs = AutoScroll && HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
                return new Size(Math.Max(0, Width - chrome - vs), Math.Max(0, Height - chrome - hs));
            }
            set => Size = new Size(value.Width + (BorderStyle == BorderStyle.None ? 0 : 2), value.Height + (BorderStyle == BorderStyle.None ? 0 : 2));
        }
        public Point Location {
            get => _location;
            set { _location = value; ApplyNativeBounds(); LocationChanged?.Invoke(this, EventArgs.Empty); }
        }
        public int Left { get => Location.X; set => Location = new Point(value, Location.Y); }
        public int Top { get => Location.Y; set => Location = new Point(Location.X, value); }
        public int Right => Location.X + Width;
        public int Bottom => Location.Y + Height;
        public Size Size {
            get => _size;
            set {
                Size clamped = ClampSize(value);
                if (_size == clamped) return;
                _size = clamped;
                ApplyNativeBounds();
                OnResize(EventArgs.Empty);
            }
        }
        public int Width { get => Size.Width; set => Size = new Size(value, Size.Height); }
        public int Height { get => Size.Height; set => Size = new Size(Size.Width, value); }
        public virtual string Text {
            get => _text;
            set { _text = value ?? ""; ApplyText(); TextChanged?.Invoke(this, EventArgs.Empty); }
        }
        public bool Visible {
            get => _visible;
            set { _visible = value; Native.IsVisible = value; VisibleChanged?.Invoke(this, EventArgs.Empty); }
        }
        public bool Enabled {
            get => _enabled;
            set { _enabled = value; Native.IsEnabled = value; OnEnabledChanged(EventArgs.Empty); }
        }
        public Color BackColor {
            get => _backColor;
            set { _backColor = value; ApplyColors(); BackColorChanged?.Invoke(this, EventArgs.Empty); }
        }
        public Color ForeColor {
            get => _foreColor;
            set { _foreColor = value; ApplyColors(); }
        }
        public Font Font { get => _font; set { _font = value; ApplyFont(); } }
        public Control? Parent {
            get => _parent;
            set {
                if (_parent == value) return;
                _parent?.Controls.Remove(this);
                value?.Controls.Add(this);
            }
        }
        public Control? TopLevelControl {
            get {
                Control c = this;
                while (c.Parent is not null) c = c.Parent;
                return c;
            }
        }
        public Form? FindForm() {
            Control? c = this;
            while (c is not null) {
                if (c is Form f) return f;
                c = c.Parent;
            }
            return null;
        }
        public Cursor Cursor {
            get => _cursor ?? Cursors.Default;
            set { _cursor = value; ApplyCursor(); }
        }
        public Image? BackgroundImage {
            get => _backgroundImage;
            set { _backgroundImage = value; OnBackgroundImageChanged(EventArgs.Empty); Invalidate(); }
        }
        public static Keys ModifierKeys {
            get {
                Keys k = Keys.None;
                if (AvaloniaBootstrap.IsKeyDown(AvKey.LeftCtrl) || AvaloniaBootstrap.IsKeyDown(AvKey.RightCtrl)) k |= Keys.Control;
                if (AvaloniaBootstrap.IsKeyDown(AvKey.LeftShift) || AvaloniaBootstrap.IsKeyDown(AvKey.RightShift)) k |= Keys.Shift;
                if (AvaloniaBootstrap.IsKeyDown(AvKey.LeftAlt) || AvaloniaBootstrap.IsKeyDown(AvKey.RightAlt)) k |= Keys.Alt;
                return k;
            }
        }
        public static MouseButtons MouseButtons => AvaloniaBootstrap.CurrentMouseButtons;
        public static Point MousePosition => AvaloniaBootstrap.ScreenMousePosition;
        public Point AutoScrollPosition { get; set; }
        public Size AutoScrollMinSize { get; set; }
        public bool AutoScroll { get; set; }
        public HScrollProperties HorizontalScroll { get; }
        public VScrollProperties VerticalScroll { get; }
        public ContextMenuStrip? ContextMenuStrip { get; set; }

        public event EventHandler? Click;
        public event EventHandler? DoubleClick;
        public event EventHandler? Load;
        public event EventHandler? Resize;
        public event EventHandler? SizeChanged;
        public event EventHandler? LocationChanged;
        public event EventHandler? Move;
        public event EventHandler? TextChanged;
        public event EventHandler? VisibleChanged;
        public event EventHandler? EnabledChanged;
        public event EventHandler? BackColorChanged;
        public event EventHandler? GotFocus;
        public event EventHandler? LostFocus;
        public event EventHandler? Enter;
        public event EventHandler? Leave;
        public event EventHandler? HandleCreated;
        public event EventHandler? HandleDestroyed;
        public event EventHandler? Disposed;
        public event MouseEventHandler? MouseDown;
        public event MouseEventHandler? MouseUp;
        public event MouseEventHandler? MouseMove;
        public event MouseEventHandler? MouseWheel;
        public event MouseEventHandler? MouseClick;
        public event MouseEventHandler? MouseDoubleClick;
        public event EventHandler? MouseEnter;
        public event EventHandler? MouseHover;
        public event EventHandler? MouseLeave;
        public event KeyEventHandler? KeyDown;
        public event KeyEventHandler? KeyUp;
        public event KeyPressEventHandler? KeyPress;
        public event PaintEventHandler? Paint;
        public event DragEventHandler? DragDrop;
        public event DragEventHandler? DragEnter;
        public event DragEventHandler? DragOver;
        public event EventHandler? DragLeave;
        public event LayoutEventHandler? Layout;
        public event ControlEventHandler? ControlAdded;
        public event ControlEventHandler? ControlRemoved;
        public event EventHandler? Validated;
        public event CancelEventHandler? Validating;
        public event PreviewKeyDownEventHandler? PreviewKeyDown;

        public Control() : this(new WfHostPanel()) { }
        internal Control(AvControl native) {
            Native = native;
            Native.Tag = this;
            Controls = new ControlCollection(this);
            HorizontalScroll = new HScrollProperties(this);
            VerticalScroll = new VScrollProperties(this);
            Native.PointerPressed += NativeOnPointerPressed;
            Native.PointerReleased += NativeOnPointerReleased;
            Native.PointerMoved += NativeOnPointerMoved;
            Native.PointerWheelChanged += NativeOnWheel;
            Native.KeyDown += NativeOnKeyDown;
            Native.KeyUp += NativeOnKeyUp;
            Native.GotFocus += (_, _) => { GotFocus?.Invoke(this, EventArgs.Empty); Enter?.Invoke(this, EventArgs.Empty); };
            Native.LostFocus += (_, _) => { LostFocus?.Invoke(this, EventArgs.Empty); Leave?.Invoke(this, EventArgs.Empty); };
            Native.DoubleTapped += (_, _) => { DoubleClick?.Invoke(this, EventArgs.Empty); MouseDoubleClick?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0)); };
            if (native is WfHostPanel host)
                host.Owner = this;
            else if (native is Avalonia.Controls.ScrollViewer sv && sv.Content is WfHostPanel inner)
                inner.Owner = this;
            ApplyColors();
            Width = 100;
            Height = 23;
        }

        internal void AttachChild(Control child, int z = -1) {
            child._parent = this;
            if (this is TabControl tabs && child is TabPage page) {
                tabs.AdoptTabPage(page);
                child.CreateControl();
                ControlAdded?.Invoke(this, new ControlEventArgs(child));
                PerformLayout();
                return;
            }
            if (HostPanel is not null) {
                if (z < 0 || z >= HostPanel.Children.Count)
                    HostPanel.Children.Add(child.Native);
                else
                    HostPanel.Children.Insert(z, child.Native);
            }
            child.CreateControl();
            ControlAdded?.Invoke(this, new ControlEventArgs(child));
            PerformLayout();
        }
        internal void DetachChild(Control child) {
            if (child._parent != this) return;
            child._parent = null;
            HostPanel?.Children.Remove(child.Native);
            ControlRemoved?.Invoke(this, new ControlEventArgs(child));
            PerformLayout();
        }

        protected virtual void CreateHandle() { IsHandleCreated = true; HandleCreated?.Invoke(this, EventArgs.Empty); }
        protected virtual void OnCreateControl() { }
        public void CreateControl() {
            CreateHandle();
            OnCreateControl();
        }
        protected void SetStyle(ControlStyles _, bool __) { }
        protected virtual CreateParams CreateParams { get; } = new();

        public void SuspendLayout() => _layoutSuspend++;
        public void ResumeLayout() => ResumeLayout(true);
        public void ResumeLayout(bool performLayout) {
            if (_layoutSuspend > 0) _layoutSuspend--;
            if (performLayout && _layoutSuspend == 0)
                PerformLayout();
        }
        public void PerformLayout() {
            if (_layoutSuspend > 0) return;
            WfLayout.Arrange(this, new Rectangle(0, 0, Math.Max(0, Width), Math.Max(0, Height)));
            HostPanel?.InvalidateArrange();
            HostPanel?.InvalidateMeasure();
            Layout?.Invoke(this, new LayoutEventArgs(this, "Layout"));
        }
        internal void PerformParentLayout() => Parent?.PerformLayout();

        public virtual Size GetPreferredSize(Size proposedSize) {
            if (Controls.Count == 0)
                return Size.IsEmpty ? new Size(20, 20) : Size;
            int r = 0, b = 0;
            foreach (Control c in Controls) {
                if (!c.Visible) continue;
                r = Math.Max(r, c.Right + c.Margin.Right);
                b = Math.Max(b, c.Bottom + c.Margin.Bottom);
            }
            return new Size(r + Padding.Horizontal, b + Padding.Vertical);
        }
        public void SetBounds(int x, int y, int width, int height) => SetBounds(x, y, width, height, BoundsSpecified.All);
        public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) {
            Point loc = Location; Size sz = Size;
            if (specified.HasFlag(BoundsSpecified.X)) loc.X = x;
            if (specified.HasFlag(BoundsSpecified.Y)) loc.Y = y;
            if (specified.HasFlag(BoundsSpecified.Width)) sz.Width = width;
            if (specified.HasFlag(BoundsSpecified.Height)) sz.Height = height;
            SetBoundsCore(loc.X, loc.Y, sz.Width, sz.Height, specified);
        }
        protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            Location = new Point(x, y);
            Size = new Size(width, height);
        }

        public void Invalidate() {
            if (HostPanel is not null)
                HostPanel.PaintOwnerToBackground();
            Native.InvalidateVisual();
        }
        public void Invalidate(Rectangle _) => Invalidate();
        public void Invalidate(bool __) => Invalidate();
        public void Refresh() { Invalidate(); Update(); }
        public void Update() { }
        public void BringToFront() {
            if (Parent?.HostPanel is { } p) {
                p.Children.Remove(Native);
                p.Children.Add(Native);
            }
        }
        public void SendToBack() {
            if (Parent?.HostPanel is { } p) {
                p.Children.Remove(Native);
                p.Children.Insert(0, Native);
            }
        }
        public bool Focus() {
            Native.Focus();
            return true;
        }
        public void Select() => Focus();
        public void Hide() => Visible = false;
        public void Show() => Visible = true;

        public Point PointToClient(Point p) {
            Point origin = PointToScreen(Point.Empty);
            return new Point(p.X - origin.X, p.Y - origin.Y);
        }
        public Point PointToScreen(Point p) {
            int x = p.X, y = p.Y;
            Control? c = this;
            while (c is not null) {
                x += c.Left;
                y += c.Top;
                if (c is Form form)
                    return new Point(x + form.Window.Position.X, y + form.Window.Position.Y);
                c = c.Parent;
            }
            return new Point(x, y);
        }
        public Rectangle RectangleToScreen(Rectangle r) {
            Point p = PointToScreen(r.Location);
            return new Rectangle(p, r.Size);
        }
        public Rectangle RectangleToClient(Rectangle r) {
            Point p = PointToClient(r.Location);
            return new Rectangle(p, r.Size);
        }

        public Graphics CreateGraphics() {
            var bmp = new Bitmap(Math.Max(1, Width), Math.Max(1, Height));
            return Graphics.FromImage(bmp);
        }

        public IAsyncResult BeginInvoke(Delegate method) => BeginInvoke(method, null);
        public IAsyncResult BeginInvoke(Delegate method, params object?[]? args) {
            Dispatcher.UIThread.Post(() => method.DynamicInvoke(args));
            return Task.CompletedTask;
        }
        public IAsyncResult BeginInvoke(Action method) {
            Dispatcher.UIThread.Post(method);
            return Task.CompletedTask;
        }
        public object? Invoke(Delegate method) => Invoke(method, null);
        public object? Invoke(Delegate method, params object?[]? args) {
            if (!InvokeRequired) return method.DynamicInvoke(args);
            return Dispatcher.UIThread.Invoke(() => method.DynamicInvoke(args));
        }
        public void Invoke(Action method) {
            if (!InvokeRequired) { method(); return; }
            Dispatcher.UIThread.Invoke(method);
        }
        public Task InvokeAsync(Action method) {
            if (!InvokeRequired) { method(); return Task.CompletedTask; }
            var tcs = new TaskCompletionSource();
            Dispatcher.UIThread.Post(() => {
                try { method(); tcs.SetResult(); }
                catch (Exception ex) { tcs.SetException(ex); }
            });
            return tcs.Task;
        }

        protected virtual void OnPaint(PaintEventArgs e) => Paint?.Invoke(this, e);
        protected virtual void OnPaintBackground(PaintEventArgs e) { }
        protected virtual void OnResize(EventArgs e) {
            Resize?.Invoke(this, e);
            SizeChanged?.Invoke(this, e);
            PerformLayout();
        }
        protected virtual void OnMouseDown(MouseEventArgs e) => MouseDown?.Invoke(this, e);
        protected virtual void OnMouseUp(MouseEventArgs e) => MouseUp?.Invoke(this, e);
        protected virtual void OnMouseMove(MouseEventArgs e) => MouseMove?.Invoke(this, e);
        protected virtual void OnMouseWheel(MouseEventArgs e) => MouseWheel?.Invoke(this, e);
        protected virtual void OnMouseClick(MouseEventArgs e) { MouseClick?.Invoke(this, e); Click?.Invoke(this, e); }
        protected virtual void OnKeyDown(KeyEventArgs e) => KeyDown?.Invoke(this, e);
        protected virtual void OnKeyUp(KeyEventArgs e) => KeyUp?.Invoke(this, e);
        protected virtual void OnClick(EventArgs e) => Click?.Invoke(this, e);
        protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);
        protected virtual void OnEnabledChanged(EventArgs e) => EnabledChanged?.Invoke(this, e);
        public event EventHandler? BackgroundImageChanged;
        protected virtual void OnBackgroundImageChanged(EventArgs e) => BackgroundImageChanged?.Invoke(this, e);
        protected virtual void OnDpiChangedAfterParent(EventArgs e) { }
        protected virtual void WndProc(ref Message m) { }
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) return;
            IsDisposed = true;
            if (disposing) {
                foreach (Control c in Controls.OfType<Control>().ToArray())
                    c.Dispose();
                HandleDestroyed?.Invoke(this, EventArgs.Empty);
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }
        public new void Dispose() { Dispose(true); GC.SuppressFinalize(this); base.Dispose(); }

        internal void RaisePaint(Graphics g) {
            var args = new PaintEventArgs(g, ClientRectangle);
            OnPaintBackground(args);
            OnPaint(args);
        }
        internal void RaiseClick() => OnClick(EventArgs.Empty);
        internal void RaiseLoad() => OnLoad(EventArgs.Empty);

        private Size ClampSize(Size s) {
            if (MinimumSize.Width > 0) s.Width = Math.Max(s.Width, MinimumSize.Width);
            if (MinimumSize.Height > 0) s.Height = Math.Max(s.Height, MinimumSize.Height);
            if (MaximumSize.Width > 0) s.Width = Math.Min(s.Width, MaximumSize.Width);
            if (MaximumSize.Height > 0) s.Height = Math.Min(s.Height, MaximumSize.Height);
            return new Size(Math.Max(0, s.Width), Math.Max(0, s.Height));
        }
        private void ApplyNativeBounds() {
            Native.Width = Width;
            Native.Height = Height;
            Avalonia.Controls.Canvas.SetLeft(Native, Left);
            Avalonia.Controls.Canvas.SetTop(Native, Top);
        }
        protected virtual void ApplyText() { }
        protected virtual void ApplyColors() { }
        protected virtual void ApplyFont() { }
        private void ApplyCursor() {
            Native.Cursor = _cursor?.Native ?? new Avalonia.Input.Cursor(StandardCursorType.Arrow);
        }
        internal static Avalonia.Media.Color ToAv(Color c) => Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

        private void NativeOnPointerPressed(object? sender, PointerPressedEventArgs e) {
            var pt = e.GetPosition(Native);
            var ev = new MouseEventArgs(MapButton(e), e.ClickCount, (int)pt.X, (int)pt.Y, 0);
            AvaloniaBootstrap.CurrentMouseButtons |= ev.Button;
            OnMouseDown(ev);
            e.Handled = false;
        }
        private void NativeOnPointerReleased(object? sender, PointerReleasedEventArgs e) {
            var pt = e.GetPosition(Native);
            var ev = new MouseEventArgs(MapButton(e), 1, (int)pt.X, (int)pt.Y, 0);
            AvaloniaBootstrap.CurrentMouseButtons &= ~ev.Button;
            OnMouseUp(ev);
            if (ev.Button == MouseButtons.Left)
                OnMouseClick(ev);
        }
        private void NativeOnPointerMoved(object? sender, PointerEventArgs e) {
            var pt = e.GetPosition(Native);
            AvaloniaBootstrap.ScreenMousePosition = PointToScreen(new Point((int)pt.X, (int)pt.Y));
            OnMouseMove(new MouseEventArgs(AvaloniaBootstrap.CurrentMouseButtons, 0, (int)pt.X, (int)pt.Y, 0));
        }
        private void NativeOnWheel(object? sender, PointerWheelEventArgs e) {
            var pt = e.GetPosition(Native);
            OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, (int)pt.X, (int)pt.Y, (int)(e.Delta.Y * 120)));
        }
        private void NativeOnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e) {
            AvaloniaBootstrap.NoteKey(e.Key, true);
            var ev = new KeyEventArgs(MapKey(e.Key) | ModifierKeys);
            OnKeyDown(ev);
            e.Handled = ev.Handled;
        }
        private void NativeOnKeyUp(object? sender, Avalonia.Input.KeyEventArgs e) {
            AvaloniaBootstrap.NoteKey(e.Key, false);
            var ev = new KeyEventArgs(MapKey(e.Key) | ModifierKeys);
            OnKeyUp(ev);
        }

        internal static MouseButtons MapButton(PointerEventArgs e) {
            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed) return MouseButtons.Left;
            if (props.IsRightButtonPressed) return MouseButtons.Right;
            if (props.IsMiddleButtonPressed) return MouseButtons.Middle;
            return MouseButtons.Left;
        }
        internal static Keys MapKey(AvKey key) => key switch {
            AvKey.LeftCtrl or AvKey.RightCtrl => Keys.ControlKey,
            AvKey.LeftShift or AvKey.RightShift => Keys.ShiftKey,
            AvKey.LeftAlt or AvKey.RightAlt => Keys.Menu,
            AvKey.Escape => Keys.Escape,
            AvKey.Space => Keys.Space,
            AvKey.Delete => Keys.Delete,
            AvKey.Left => Keys.Left,
            AvKey.Right => Keys.Right,
            AvKey.Up => Keys.Up,
            AvKey.Down => Keys.Down,
            AvKey.Enter => Keys.Enter,
            AvKey.Tab => Keys.Tab,
            AvKey.A => Keys.A, AvKey.C => Keys.C, AvKey.V => Keys.V, AvKey.X => Keys.X,
            AvKey.W => Keys.W, AvKey.S => Keys.S, AvKey.D => Keys.D,
            _ => Enum.TryParse(key.ToString(), true, out Keys k) ? k : Keys.None
        };

        public class ControlCollection : IList, IEnumerable<Control> {
            private readonly Control _owner;
            private readonly List<Control> _items = [];
            internal readonly Dictionary<Control, (int Col, int Row)> TableCells = [];
            public ControlCollection(Control owner) { _owner = owner; }
            public int Count => _items.Count;
            public Control this[int index] => _items[index];
            public Control? this[string name] => _items.FirstOrDefault(c => c.Name == name);
            public void Add(Control value) {
                if (_items.Contains(value)) return;
                _items.Add(value);
                _owner.AttachChild(value);
            }
            public void Add(Control value, int column, int row) {
                TableCells[value] = (column, row);
                Add(value);
            }
            public void AddRange(Control[] controls) { foreach (var c in controls) Add(c); }
            public void Remove(Control value) {
                if (!_items.Remove(value)) return;
                TableCells.Remove(value);
                _owner.DetachChild(value);
            }
            public void Clear() {
                foreach (var c in _items.ToArray()) Remove(c);
            }
            public bool Contains(Control c) => _items.Contains(c);
            public int IndexOf(Control c) => _items.IndexOf(c);
            public void SetChildIndex(Control c, int index) {
                _items.Remove(c);
                _items.Insert(Math.Clamp(index, 0, _items.Count), c);
            }
            public int GetChildIndex(Control c) => IndexOf(c);
            public Control[] Find(string name, bool searchAllChildren) {
                var list = new List<Control>();
                FindInto(name, searchAllChildren, list);
                return [.. list];
            }
            private void FindInto(string name, bool all, List<Control> list) {
                foreach (Control c in _items) {
                    if (c.Name == name) list.Add(c);
                    if (all) c.Controls.FindInto(name, true, list);
                }
            }
            public IEnumerator<Control> GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            bool IList.IsFixedSize => false;
            bool IList.IsReadOnly => false;
            object ICollection.SyncRoot => this;
            bool ICollection.IsSynchronized => false;
            object? IList.this[int index] { get => this[index]; set { } }
            int IList.Add(object? value) { if (value is Control c) { Add(c); return Count - 1; } return -1; }
            bool IList.Contains(object? value) => value is Control c && Contains(c);
            int IList.IndexOf(object? value) => value is Control c ? IndexOf(c) : -1;
            void IList.Insert(int index, object? value) { if (value is Control c) { _items.Insert(index, c); _owner.AttachChild(c, index); } }
            void IList.Remove(object? value) { if (value is Control c) Remove(c); }
            void IList.RemoveAt(int index) => Remove(_items[index]);
            void IList.Clear() => Clear();
            void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
        }
    }

    public interface IDropTarget { }

    public class ScrollProperties {
        protected Control Owner { get; }
        public bool Visible { get; set; }
        public int Value { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; } = 100;
        public int LargeChange { get; set; } = 10;
        public int SmallChange { get; set; } = 1;
        public bool Enabled { get; set; } = true;
        internal ScrollProperties(Control owner) { Owner = owner; }
    }
    public class HScrollProperties : ScrollProperties { public HScrollProperties(Control o) : base(o) { } }
    public class VScrollProperties : ScrollProperties { public VScrollProperties(Control o) : base(o) { } }

    internal sealed class WfHostPanel : AvPanel {
        public Control? Owner { get; set; }
        private readonly Avalonia.Controls.Image _paintLayer = new() { Stretch = Avalonia.Media.Stretch.Fill, IsHitTestVisible = false };

        public WfHostPanel() {
            Children.Add(_paintLayer);
        }

        protected override AvSize MeasureOverride(AvSize availableSize) {
            foreach (AvControl child in Children)
                child.Measure(availableSize);
            if (Owner is { AutoSize: true } o) {
                Size pref = o.GetPreferredSize(Size.Empty);
                return new AvSize(pref.Width, pref.Height);
            }
            double w = double.IsInfinity(availableSize.Width) ? Owner?.Width ?? 0 : availableSize.Width;
            double h = double.IsInfinity(availableSize.Height) ? Owner?.Height ?? 0 : availableSize.Height;
            return new AvSize(Math.Max(0, w), Math.Max(0, h));
        }

        protected override AvSize ArrangeOverride(AvSize finalSize) {
            if (Owner is null) return finalSize;
            if ((int)finalSize.Width != Owner.Width || (int)finalSize.Height != Owner.Height)
                Owner.Size = new Size((int)finalSize.Width, (int)finalSize.Height);
            WfLayout.Arrange(Owner, new Rectangle(0, 0, (int)finalSize.Width, (int)finalSize.Height));
            _paintLayer.Arrange(new Avalonia.Rect(0, 0, finalSize.Width, finalSize.Height));
            foreach (AvControl child in Children) {
                if (child.Tag is not Control wf) continue;
                child.Arrange(new Avalonia.Rect(wf.Left, wf.Top, Math.Max(0, wf.Width), Math.Max(0, wf.Height)));
            }
            if (Owner is UserControl)
                PaintOwnerToBackground();
            return finalSize;
        }

        internal void PaintOwnerToBackground() {
            if (Owner is null) return;
            int w = Math.Max(1, Owner.Width);
            int h = Math.Max(1, Owner.Height);
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
                Owner.RaisePaint(g);
            using var ms = new System.IO.MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            bmp.Dispose();
            ms.Position = 0;
            _paintLayer.Source = new Avalonia.Media.Imaging.Bitmap(ms);
        }
    }
}
