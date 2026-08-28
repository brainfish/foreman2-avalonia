using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
    [Flags] public enum Keys {
        None = 0, LButton = 1, RButton = 2, Cancel = 3, MButton = 4,
        Back = 8, Tab = 9, LineFeed = 10, Clear = 12, Return = 13, Enter = 13,
        ShiftKey = 16, ControlKey = 17, Menu = 18, Pause = 19, CapsLock = 20,
        Escape = 27, Space = 32, PageUp = 33, PageDown = 34, End = 35, Home = 36,
        Left = 37, Up = 38, Right = 39, Down = 40, Select = 41, Print = 42,
        Execute = 43, PrintScreen = 44, Insert = 45, Delete = 46, Help = 47,
        D0 = 48, D1 = 49, D2 = 50, D3 = 51, D4 = 52, D5 = 53, D6 = 54, D7 = 55, D8 = 56, D9 = 57,
        A = 65, B = 66, C = 67, D = 68, E = 69, F = 70, G = 71, H = 72, I = 73, J = 74, K = 75, L = 76, M = 77,
        N = 78, O = 79, P = 80, Q = 81, R = 82, S = 83, T = 84, U = 85, V = 86, W = 87, X = 88, Y = 89, Z = 90,
        LWin = 91, RWin = 92, Apps = 93, Sleep = 95,
        NumPad0 = 96, NumPad1 = 97, NumPad2 = 98, NumPad3 = 99, NumPad4 = 100, NumPad5 = 101, NumPad6 = 102, NumPad7 = 103, NumPad8 = 104, NumPad9 = 105,
        Multiply = 106, Add = 107, Separator = 108, Subtract = 109, Decimal = 110, Divide = 111,
        F1 = 112, F2 = 113, F3 = 114, F4 = 115, F5 = 116, F6 = 117, F7 = 118, F8 = 119, F9 = 120, F10 = 121, F11 = 122, F12 = 123,
        NumLock = 144, Scroll = 145,
        Shift = 0x10000, Control = 0x20000, Alt = 0x40000,
        KeyCode = 0xFFFF, Modifiers = unchecked((int)0xFFFF0000)
    }
    [Flags] public enum MouseButtons { None = 0, Left = 1048576, Right = 2097152, Middle = 4194304, XButton1 = 8388608, XButton2 = 16777216 }
    public enum DialogResult { None = 0, OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7 }
    public enum MessageBoxButtons { OK = 0, OKCancel = 1, AbortRetryIgnore = 2, YesNoCancel = 3, YesNo = 4, RetryCancel = 5 }
    public enum MessageBoxIcon { None = 0, Hand = 16, Stop = 16, Error = 16, Question = 32, Exclamation = 48, Warning = 48, Asterisk = 64, Information = 64 }
    public enum DockStyle { None = 0, Top = 1, Bottom = 2, Left = 3, Right = 4, Fill = 5 }
    [Flags] public enum AnchorStyles { None = 0, Top = 1, Bottom = 2, Left = 4, Right = 8 }
    public enum BorderStyle { None = 0, FixedSingle = 1, Fixed3D = 2 }
    public enum FlatStyle { Flat = 0, Popup = 1, Standard = 2, System = 3 }
    public enum FormBorderStyle { None = 0, FixedSingle = 1, Fixed3D = 2, FixedDialog = 3, Sizable = 4, FixedToolWindow = 5, SizableToolWindow = 6 }
    public enum FormStartPosition { Manual = 0, CenterScreen = 1, WindowsDefaultLocation = 2, WindowsDefaultBounds = 3, CenterParent = 4 }
    public enum FormWindowState { Normal = 0, Minimized = 1, Maximized = 2 }
    public enum AutoScaleMode { None = 0, Font = 1, Dpi = 2, Inherit = 3 }
    public enum AutoSizeMode { GrowAndShrink = 0, GrowOnly = 1 }
    public enum SizeType { AutoSize = 0, Absolute = 1, Percent = 2 }
    public enum FlowDirection { LeftToRight = 0, TopDown = 1, RightToLeft = 2, BottomUp = 3 }
    public enum PictureBoxSizeMode { Normal = 0, StretchImage = 1, AutoSize = 2, CenterImage = 3, Zoom = 4 }
    public enum HorizontalAlignment { Left = 0, Right = 1, Center = 2 }
    public enum CheckState { Unchecked = 0, Checked = 1, Indeterminate = 2 }
    public enum SelectionMode { None = 0, One = 1, MultiSimple = 2, MultiExtended = 3 }
    public enum View { LargeIcon = 0, Details = 1, SmallIcon = 2, List = 3, Tile = 4 }
    public enum ColumnHeaderStyle { None = 0, Nonclickable = 1, Clickable = 2 }
    public enum SortOrder { None = 0, Ascending = 1, Descending = 2 }
    public enum ColorDepth { Depth4Bit = 4, Depth8Bit = 8, Depth16Bit = 16, Depth24Bit = 24, Depth32Bit = 32 }
    public enum ComboBoxStyle { Simple = 0, DropDown = 1, DropDownList = 2 }
    public enum DrawMode { Normal = 0, OwnerDrawFixed = 1, OwnerDrawVariable = 2 }
    public enum ImageLayout { None = 0, Tile = 1, Center = 2, Stretch = 3, Zoom = 4 }
    public enum CloseReason { None = 0, WindowsShutDown = 1, MdiFormClosing = 2, UserClosing = 3, TaskManagerClosing = 4, FormOwnerClosing = 5, ApplicationExitCall = 6 }
    public enum ToolStripDropDownCloseReason { AppFocusChange = 0, AppClicked = 1, ItemClicked = 2, Keyboard = 3, CloseCalled = 4 }
    public enum HighDpiMode { DpiUnaware = 0, SystemAware = 1, PerMonitor = 2, PerMonitorV2 = 3, DpiUnawareGdiScaled = 4 }
    public enum ImeMode { NoControl = 0, On = 1, Off = 2, Disable = 3, Hiragana = 4, Katakana = 5, KatakanaHalf = 6, AlphaFull = 7, Alpha = 8, HangulFull = 9, Hangul = 10, Inherit = -1, Close = 11, OnHalf = 12 }
    [Flags] public enum ControlStyles {
        ContainerControl = 1, UserPaint = 2, Opaque = 4, ResizeRedraw = 16, FixedWidth = 32, FixedHeight = 64,
        StandardClick = 256, Selectable = 512, UserMouse = 1024, SupportsTransparentBackColor = 2048,
        StandardDoubleClick = 4096, AllPaintingInWmPaint = 8192, CacheText = 16384, EnableNotifyMessage = 32768,
        DoubleBuffer = 65536, OptimizedDoubleBuffer = 131072, UseTextForAccessibility = 262144
    }
    [Flags] public enum BoundsSpecified { None = 0, X = 1, Y = 2, Width = 4, Height = 8, Location = 3, Size = 12, All = 15 }
    [Flags] public enum DragDropEffects { None = 0, Copy = 1, Move = 2, Link = 4, Scroll = int.MinValue, All = -2147483645 }
    public enum Appearance { Normal = 0, Button = 1 }
    public enum RightToLeft { No = 0, Yes = 1, Inherit = 2 }
    public enum PaddingMode { None }
    public enum TabAlignment { Top = 0, Bottom = 1, Left = 2, Right = 3 }
    public enum TabSizeMode { Normal = 0, FillToRight = 1, Fixed = 2 }
    public enum FixedPanel { None = 0, Panel1 = 1, Panel2 = 2 }
    public enum Orientation { Horizontal = 0, Vertical = 1 }
    public enum TickStyle { None = 0, TopLeft = 1, BottomRight = 2, Both = 3 }
    public enum DataSourceUpdateMode { OnValidation = 0, OnPropertyChanged = 1, Never = 2 }

    public readonly struct Padding : IEquatable<Padding> {
        public static readonly Padding Empty = new(0);
        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Horizontal => Left + Right;
        public int Vertical => Top + Bottom;
        public Size Size => new(Horizontal, Vertical);
        public static implicit operator Padding(Point p) => new(p.X, p.Y, p.X, p.Y);
        public Padding(int all) { Left = Top = Right = Bottom = all; }
        public Padding(int left, int top, int right, int bottom) { Left = left; Top = top; Right = right; Bottom = bottom; }
        public bool Equals(Padding other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        public override bool Equals(object? obj) => obj is Padding p && Equals(p);
        public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
        public static bool operator ==(Padding a, Padding b) => a.Equals(b);
        public static bool operator !=(Padding a, Padding b) => !a.Equals(b);
    }

    public sealed class ColumnStyle {
        public SizeType SizeType { get; set; }
        public float Width { get; set; }
        public ColumnStyle() : this(SizeType.AutoSize, 0) { }
        public ColumnStyle(SizeType sizeType) : this(sizeType, 0) { }
        public ColumnStyle(SizeType sizeType, float width) { SizeType = sizeType; Width = width; }
    }
    public sealed class RowStyle {
        public SizeType SizeType { get; set; }
        public float Height { get; set; }
        public RowStyle() : this(SizeType.AutoSize, 0) { }
        public RowStyle(SizeType sizeType) : this(sizeType, 0) { }
        public RowStyle(SizeType sizeType, float height) { SizeType = sizeType; Height = height; }
    }

    public class MouseEventArgs : EventArgs {
        public MouseButtons Button { get; }
        public int Clicks { get; }
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
        public Point Location => new(X, Y);
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) {
            Button = button; Clicks = clicks; X = x; Y = y; Delta = delta;
        }
    }
    public class KeyEventArgs : EventArgs {
        public Keys KeyData { get; }
        public Keys KeyCode => KeyData & Keys.KeyCode;
        public Keys Modifiers => KeyData & Keys.Modifiers;
        public bool Control => (KeyData & Keys.Control) != 0;
        public bool Shift => (KeyData & Keys.Shift) != 0;
        public bool Alt => (KeyData & Keys.Alt) != 0;
        public bool Handled { get; set; }
        public bool SuppressKeyPress { get; set; }
        public KeyEventArgs(Keys keyData) { KeyData = keyData; }
    }
    public class KeyPressEventArgs : EventArgs {
        public char KeyChar { get; set; }
        public bool Handled { get; set; }
        public KeyPressEventArgs(char keyChar) { KeyChar = keyChar; }
    }
    public class PaintEventArgs : EventArgs, IDisposable {
        public Graphics Graphics { get; }
        public Rectangle ClipRectangle { get; }
        public PaintEventArgs(Graphics graphics, Rectangle clipRect) { Graphics = graphics; ClipRectangle = clipRect; }
        public void Dispose() { }
    }
    public class DrawItemEventArgs : EventArgs, IDisposable {
        public Graphics Graphics { get; }
        public Rectangle Bounds { get; }
        public int Index { get; }
        public Font? Font { get; }
        public DrawItemEventArgs(Graphics graphics, Font? font, Rectangle rect, int index, DrawItemState _) {
            Graphics = graphics; Font = font; Bounds = rect; Index = index;
        }
        public void DrawBackground() { }
        public void Dispose() { }
    }
    [Flags] public enum DrawItemState { None = 0, Selected = 1, Disabled = 4, Focused = 16 }
    public class ItemCheckEventArgs : EventArgs {
        public int Index { get; }
        public CheckState NewValue { get; set; }
        public CheckState CurrentValue { get; }
        public ItemCheckEventArgs(int index, CheckState newValue, CheckState currentValue) { Index = index; NewValue = newValue; CurrentValue = currentValue; }
    }
    public class ItemCheckedEventArgs : EventArgs {
        public ListViewItem Item { get; }
        public ItemCheckedEventArgs(ListViewItem item) { Item = item; }
    }
    public class ListViewItemSelectionChangedEventArgs : EventArgs {
        public ListViewItem Item { get; }
        public int ItemIndex { get; }
        public bool IsSelected { get; }
        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected) {
            Item = item; ItemIndex = itemIndex; IsSelected = isSelected;
        }
    }
    public class FormClosingEventArgs : CancelEventArgs {
        public CloseReason CloseReason { get; }
        public FormClosingEventArgs(CloseReason closeReason, bool cancel) : base(cancel) { CloseReason = closeReason; }
    }
    public class FormClosedEventArgs : EventArgs {
        public CloseReason CloseReason { get; }
        public FormClosedEventArgs(CloseReason closeReason) { CloseReason = closeReason; }
    }
    public class ToolStripDropDownClosingEventArgs : CancelEventArgs {
        public ToolStripDropDownCloseReason CloseReason { get; }
        public ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason reason) { CloseReason = reason; }
    }
    public class PopupEventArgs : CancelEventArgs {
        public Size ToolTipSize { get; set; }
        public Control? AssociatedControl { get; }
        public PopupEventArgs() { }
        public PopupEventArgs(Control? associatedWindow, Control? associatedControl, bool isBalloon, Size size) {
            AssociatedControl = associatedControl; ToolTipSize = size;
        }
    }
    public class DrawToolTipEventArgs : EventArgs {
        public Graphics Graphics { get; }
        public Rectangle Bounds { get; }
        public string? ToolTipText { get; }
        public Control? AssociatedControl { get; }
        public DrawToolTipEventArgs(Graphics g, Control? associatedWindow, Control? associatedControl, Rectangle bounds, string? text, Color _, Color __, Font? ___) {
            Graphics = g; Bounds = bounds; ToolTipText = text; AssociatedControl = associatedControl;
        }
        public void DrawBackground() { Graphics.FillRectangle(new SolidBrush(Color.FromArgb(65, 65, 65)), Bounds); }
        public void DrawBorder() { }
        public void DrawText() { }
    }
    public class DragEventArgs : EventArgs {
        public IDataObject? Data { get; }
        public int X { get; }
        public int Y { get; }
        public DragDropEffects AllowedEffect { get; }
        public DragDropEffects Effect { get; set; }
        public DragEventArgs(IDataObject? data, int keyState, int x, int y, DragDropEffects allowed, DragDropEffects effect) {
            Data = data; X = x; Y = y; AllowedEffect = allowed; Effect = effect;
        }
    }
    public class GiveFeedbackEventArgs : EventArgs {
        public DragDropEffects Effect { get; }
        public bool UseDefaultCursors { get; set; } = true;
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefault) { Effect = effect; UseDefaultCursors = useDefault; }
    }
    public class QueryContinueDragEventArgs : EventArgs {
        public int KeyState { get; }
        public bool EscapePressed { get; }
        public DragAction Action { get; set; }
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action) { KeyState = keyState; EscapePressed = escapePressed; Action = action; }
    }
    public enum DragAction { Continue = 0, Drop = 1, Cancel = 2 }
    public class ScrollEventArgs : EventArgs {
        public int NewValue { get; set; }
        public ScrollEventArgs(ScrollEventType _, int newValue) { NewValue = newValue; }
    }
    public enum ScrollEventType { SmallDecrement = 0, ThumbTrack = 5, EndScroll = 8 }
    public class TreeViewEventArgs : EventArgs { }
    public class NodeLabelEditEventArgs : EventArgs { }
    public class LinkLabelLinkClickedEventArgs : EventArgs { }
    public class DataGridViewCellEventArgs : EventArgs { }
    public class ConvertEventArgs : EventArgs { }
    public class HandledMouseEventArgs : MouseEventArgs {
        public bool Handled { get; set; }
        public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) : base(button, clicks, x, y, delta) { }
    }
    public class PreviewKeyDownEventArgs : EventArgs {
        public Keys KeyData { get; }
        public bool IsInputKey { get; set; }
        public PreviewKeyDownEventArgs(Keys keyData) { KeyData = keyData; }
    }
    public class HelpEventArgs : EventArgs {
        public Point MousePos { get; }
        public bool Handled { get; set; }
        public HelpEventArgs(Point mousePos) { MousePos = mousePos; }
    }
    public class InvalidateEventArgs : EventArgs {
        public Rectangle InvalidRect { get; }
        public InvalidateEventArgs(Rectangle invalidRect) { InvalidRect = invalidRect; }
    }
    public class LayoutEventArgs : EventArgs {
        public Control? AffectedControl { get; }
        public string? AffectedProperty { get; }
        public LayoutEventArgs(IComponent? affected, string? property) { AffectedControl = affected as Control; AffectedProperty = property; }
    }
    public class ControlEventArgs : EventArgs {
        public Control Control { get; }
        public ControlEventArgs(Control control) { Control = control; }
    }
    public class SplitterEventArgs : EventArgs {
        public int SplitX { get; set; }
        public int SplitY { get; set; }
        public SplitterEventArgs(int x, int y, int splitX, int splitY) { SplitX = splitX; SplitY = splitY; }
    }
    public class TabControlEventArgs : EventArgs {
        public TabPage? TabPage { get; }
        public int TabPageIndex { get; }
        public TabControlEventArgs(TabPage? page, int index, TabControlAction _) { TabPage = page; TabPageIndex = index; }
    }
    public enum TabControlAction { Selecting = 0, Selected = 1, Deselecting = 2, Deselected = 3 }

    public delegate void MouseEventHandler(object? sender, MouseEventArgs e);
    public delegate void KeyEventHandler(object? sender, KeyEventArgs e);
    public delegate void KeyPressEventHandler(object? sender, KeyPressEventArgs e);
    public delegate void PaintEventHandler(object? sender, PaintEventArgs e);
    public delegate void DrawItemEventHandler(object? sender, DrawItemEventArgs e);
    public delegate void FormClosingEventHandler(object? sender, FormClosingEventArgs e);
    public delegate void FormClosedEventHandler(object? sender, FormClosedEventArgs e);
    public delegate void PopupEventHandler(object? sender, PopupEventArgs e);
    public delegate void DrawToolTipEventHandler(object? sender, DrawToolTipEventArgs e);
    public delegate void DragEventHandler(object? sender, DragEventArgs e);
    public delegate void ItemCheckEventHandler(object? sender, ItemCheckEventArgs e);
    public delegate void ItemCheckedEventHandler(object? sender, ItemCheckedEventArgs e);
    public delegate void ListViewItemSelectionChangedEventHandler(object? sender, ListViewItemSelectionChangedEventArgs e);
    public delegate void ScrollEventHandler(object? sender, ScrollEventArgs e);
    public delegate void PreviewKeyDownEventHandler(object? sender, PreviewKeyDownEventArgs e);
    public delegate void HelpEventHandler(object? sender, HelpEventArgs e);
    public delegate void LayoutEventHandler(object? sender, LayoutEventArgs e);
    public delegate void ControlEventHandler(object? sender, ControlEventArgs e);
    public delegate void SplitterEventHandler(object? sender, SplitterEventArgs e);
    public delegate void TabControlEventHandler(object? sender, TabControlEventArgs e);
    public delegate void GiveFeedbackEventHandler(object? sender, GiveFeedbackEventArgs e);
    public delegate void QueryContinueDragEventHandler(object? sender, QueryContinueDragEventArgs e);
    public delegate void ToolStripItemClickedEventHandler(object? sender, ToolStripItemClickedEventArgs e);
    public delegate void ToolStripDropDownClosingEventHandler(object? sender, ToolStripDropDownClosingEventArgs e);
    public class ToolStripItemClickedEventArgs : EventArgs {
        public ToolStripItem ClickedItem { get; }
        public ToolStripItemClickedEventArgs(ToolStripItem item) { ClickedItem = item; }
    }

    public struct Message {
        public IntPtr HWnd;
        public int Msg;
        public IntPtr WParam;
        public IntPtr LParam;
        public IntPtr Result;
    }
    public class CreateParams {
        public int ExStyle { get; set; }
        public int Style { get; set; }
        public string? Caption { get; set; }
        public string? ClassName { get; set; }
        public IntPtr Parent { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public interface IWin32Window { IntPtr Handle { get; } }
    public interface IDataObject {
        object? GetData(string format);
        object? GetData(Type format);
        bool GetDataPresent(string format);
        bool GetDataPresent(Type format);
        void SetData(string format, object data);
        void SetData(object data);
        string[] GetFormats();
    }
    public class DataObject : IDataObject {
        private readonly Dictionary<string, object> _data = new(StringComparer.OrdinalIgnoreCase);
        public DataObject() { }
        public DataObject(object data) { SetData(data); }
        public object? GetData(string format) => _data.TryGetValue(format, out var d) ? d : null;
        public object? GetData(Type format) => GetData(format.FullName ?? "");
        public bool GetDataPresent(string format) => _data.ContainsKey(format);
        public bool GetDataPresent(Type format) => GetDataPresent(format.FullName ?? "");
        public void SetData(string format, object data) => _data[format] = data;
        public void SetData(object data) => _data[data.GetType().FullName ?? "object"] = data;
        public string[] GetFormats() => [.. _data.Keys];
    }

    public static class SystemInformation {
        public static int VerticalScrollBarWidth => 16;
        public static int HorizontalScrollBarHeight => 16;
        public static bool TerminalServerSession => false;
        public static Size CaptionButtonSize => new(16, 16);
        public static int CaptionHeight => 30;
        public static int ToolWindowCaptionHeight => 22;
        public static Size DragSize => new(4, 4);
        public static int MouseHoverTime => 400;
        public static Size MouseHoverSize => new(4, 4);
    }
    public static class TextRenderer {
        public static Size MeasureText(string? text, Font? font) {
            if (string.IsNullOrEmpty(text)) return Size.Empty;
            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);
            SizeF s = g.MeasureString(text, font ?? SystemFonts.DefaultFont);
            return Size.Ceiling(s);
        }
        public static Size MeasureText(string? text, Font? font, Size proposed, TextFormatFlags _) => MeasureText(text, font);
        public static Size MeasureText(string? text, Font? font, Size proposed) => MeasureText(text, font);
        public static Size MeasureText(IDeviceContext? _, string? text, Font? font) => MeasureText(text, font);
        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color color) { }
        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color color) { }
    }
    public interface IDeviceContext { IntPtr GetHdc(); void ReleaseHdc(); }
}

namespace System.Windows.Forms.VisualStyles {
    public enum CheckBoxState { UncheckedNormal = 1, CheckedNormal = 5, MixedNormal = 9 }
    public static class CheckBoxRenderer {
        public static Size GetGlyphSize(Graphics _, CheckBoxState __) => new(13, 13);
        public static void DrawCheckBox(Graphics g, Point glyphLocation, CheckBoxState state) {
            var r = new Rectangle(glyphLocation.X, glyphLocation.Y, 13, 13);
            g.DrawRectangle(Pens.Black, r);
            if (state == CheckBoxState.CheckedNormal) {
                g.DrawLine(Pens.Black, r.Left + 2, r.Top + 6, r.Left + 5, r.Bottom - 3);
                g.DrawLine(Pens.Black, r.Left + 5, r.Bottom - 3, r.Right - 2, r.Top + 3);
            }
        }
    }
}
namespace System.Windows.Forms {
    public static class ProgressBarRenderer {
        public static void DrawHorizontalBar(Graphics g, Rectangle bounds) => g.DrawRectangle(Pens.Black, bounds);
        public static void DrawHorizontalChunks(Graphics g, Rectangle bounds) => g.FillRectangle(Brushes.Green, bounds);
        public static bool IsSupported => true;
    }
}
