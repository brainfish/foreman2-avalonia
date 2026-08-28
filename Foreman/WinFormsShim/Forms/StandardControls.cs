using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Controls;
using Avalonia.Layout;
using Image = System.Drawing.Image;
using AvButton = Avalonia.Controls.Button;
using AvControl = Avalonia.Controls.Control;
using AvTextBlock = Avalonia.Controls.TextBlock;
using AvTextBox = Avalonia.Controls.TextBox;
using AvCheckBox = Avalonia.Controls.CheckBox;
using AvComboBox = Avalonia.Controls.ComboBox;
using AvRadio = Avalonia.Controls.RadioButton;
using AvImage = Avalonia.Controls.Image;
using AvBorder = Avalonia.Controls.Border;
using AvProgress = Avalonia.Controls.ProgressBar;
using AvNumeric = Avalonia.Controls.NumericUpDown;
using AvScrollBar = Avalonia.Controls.Primitives.ScrollBar;
using AvListBox = Avalonia.Controls.ListBox;
using AvStack = Avalonia.Controls.StackPanel;

namespace System.Windows.Forms {
    public partial class Button : Control, IButtonControl {
        private readonly AvButton _btn;
        public DialogResult DialogResult { get; set; }
        public FlatStyle FlatStyle { get; set; }
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;
        public bool AutoEllipsis { get; set; }
        public Image? Image { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public int ImageIndex { get; set; } = -1;
        public Button() : base(Make(out AvButton b)) {
            _btn = b;
            _btn.Click += (_, _) => {
                RaiseClick();
                if (DialogResult != DialogResult.None && FindForm() is Form f)
                    f.DialogResult = DialogResult;
            };
        }
        private static AvControl Make(out AvButton b) { b = new AvButton { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch }; return b; }
        protected override void ApplyText() => _btn.Content = Text;
        public override Size GetPreferredSize(Size proposedSize) =>
            new(Math.Max(75, (int)(Text.Length * 7) + 16), 28);
        public void NotifyDefault(bool value) { }
        public void PerformClick() => RaiseClick();
    }
    public class Label : Control {
        private readonly AvTextBlock _tb;
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.TopLeft;
        public bool AutoEllipsis { get; set; }
        public bool UseMnemonic { get; set; } = true;
        private static AvControl Make(out AvTextBlock t) { t = new AvTextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap }; return t; }
        public Label() : base(Make(out AvTextBlock t)) { _tb = t; AutoSize = true; Height = 15; }
        protected override void ApplyText() => _tb.Text = Text;
        protected override void ApplyColors() { _tb.Foreground = new Avalonia.Media.SolidColorBrush(ToAv(ForeColor)); }
        public override Size GetPreferredSize(Size proposedSize) {
            Size s = TextRenderer.MeasureText(Text, Font);
            return new Size(s.Width + 4, s.Height + 2);
        }
    }
    public class TextBox : Control {
        private readonly AvTextBox _tb;
        public bool ReadOnly { get => _tb.IsReadOnly; set => _tb.IsReadOnly = value; }
        public bool Multiline { get; set; }
        public bool WordWrap { get; set; } = true;
        public HorizontalAlignment TextAlign { get; set; }
        public bool UseSystemPasswordChar { get; set; }
        public char PasswordChar { get; set; }
        public int MaxLength { get => _tb.MaxLength; set => _tb.MaxLength = value; }
        public int SelectionStart { get => _tb.SelectionStart; set => _tb.SelectionStart = value; }
        public int SelectionLength { get => _tb.SelectionEnd - _tb.SelectionStart; set => _tb.SelectionEnd = _tb.SelectionStart + value; }
        public ScrollBars ScrollBars { get; set; }
        public BorderStyle BorderStyle { get; set; } = BorderStyle.Fixed3D;
        public event EventHandler? TextChanged;
        public TextBox() : base(Make(out AvTextBox t)) {
            _tb = t;
            _tb.TextChanged += (_, _) => { base.Text = _tb.Text ?? ""; TextChanged?.Invoke(this, EventArgs.Empty); };
        }
        private static AvControl Make(out AvTextBox t) { t = new AvTextBox(); return t; }
        public override string Text { get => _tb.Text ?? ""; set { _tb.Text = value; } }
        public void SelectAll() => _tb.SelectAll();
        public void Clear() => Text = "";
        public void AppendText(string t) => Text += t;
    }
    public enum ScrollBars { None = 0, Horizontal = 1, Vertical = 2, Both = 3 }
    public class CheckBox : Control {
        private readonly AvCheckBox _cb;
        public bool Checked { get => _cb.IsChecked == true; set { _cb.IsChecked = value; } }
        public CheckState CheckState {
            get => _cb.IsChecked switch { true => CheckState.Checked, false => CheckState.Unchecked, _ => CheckState.Indeterminate };
            set => _cb.IsChecked = value == CheckState.Checked ? true : value == CheckState.Unchecked ? false : null;
        }
        public Appearance Appearance { get; set; }
        public ContentAlignment CheckAlign { get; set; }
        public bool ThreeState { get => _cb.IsThreeState; set => _cb.IsThreeState = value; }
        public bool AutoCheck { get; set; } = true;
        public event EventHandler? CheckedChanged;
        public event EventHandler? CheckStateChanged;
        public CheckBox() : base(Make(out AvCheckBox c)) {
            _cb = c; AutoSize = true;
            _cb.IsCheckedChanged += (_, _) => { CheckedChanged?.Invoke(this, EventArgs.Empty); CheckStateChanged?.Invoke(this, EventArgs.Empty); };
        }
        private static AvControl Make(out AvCheckBox c) { c = new AvCheckBox(); return c; }
        protected override void ApplyText() => _cb.Content = Text;
        public override Size GetPreferredSize(Size proposedSize) => new(Math.Max(20, TextRenderer.MeasureText(Text, Font).Width + 24), 22);
    }
    public class RadioButton : Control {
        private readonly AvRadio _rb;
        public bool Checked { get => _rb.IsChecked == true; set => _rb.IsChecked = value; }
        public Appearance Appearance { get; set; }
        public event EventHandler? CheckedChanged;
        public RadioButton() : base(Make(out AvRadio r)) {
            _rb = r; AutoSize = true;
            _rb.IsCheckedChanged += (_, _) => CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
        private static AvControl Make(out AvRadio r) { r = new AvRadio(); return r; }
        protected override void ApplyText() => _rb.Content = Text;
        public override Size GetPreferredSize(Size proposedSize) => new(Math.Max(20, TextRenderer.MeasureText(Text, Font).Width + 24), 22);
    }
    public class ComboBox : Control {
        private readonly AvComboBox _cb;
        public ComboBoxStyle DropDownStyle { get; set; }
        public string DisplayMember { get; set; } = "";
        public bool FormattingEnabled { get; set; }
        public DrawMode DrawMode { get; set; }
        public int DropDownWidth { get; set; } = 100;
        public int ItemHeight { get; set; } = 15;
        public ComboBox.ObjectCollection Items { get; }
        public int SelectedIndex {
            get => _cb.SelectedIndex;
            set { _cb.SelectedIndex = value; SelectedIndexChanged?.Invoke(this, EventArgs.Empty); }
        }
        public object? SelectedItem {
            get => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null;
            set { int i = Items.IndexOf(value); if (i >= 0) SelectedIndex = i; }
        }
        public string SelectedText { get => SelectedItem?.ToString() ?? ""; set { } }
        public event EventHandler? SelectedIndexChanged;
        public event EventHandler? SelectedValueChanged;
        public event DrawItemEventHandler? DrawItem;
        public ComboBox() : base(Make(out AvComboBox c)) {
            _cb = c;
            Items = new ObjectCollection(this);
            _cb.SelectionChanged += (_, _) => { SelectedIndexChanged?.Invoke(this, EventArgs.Empty); SelectedValueChanged?.Invoke(this, EventArgs.Empty); };
        }
        private static AvControl Make(out AvComboBox c) { c = new AvComboBox(); return c; }
        public override string Text {
            get => _cb.SelectionBoxItem?.ToString() ?? base.Text;
            set { base.Text = value; _cb.PlaceholderText = value; }
        }
        internal void SyncItems() {
            _cb.Items.Clear();
            foreach (var i in Items) _cb.Items.Add(ListBox.FormatItem(i, DisplayMember));
        }
        public class ObjectCollection : IList {
            private readonly ComboBox _owner;
            private readonly List<object?> _items = [];
            public ObjectCollection(ComboBox owner) { _owner = owner; }
            public int Count => _items.Count;
            public object? this[int index] { get => _items[index]; set => _items[index] = value; }
            public int Add(object? item) { _items.Add(item); _owner.SyncItems(); return _items.Count - 1; }
            public void AddRange(object[] items) { _items.AddRange(items); _owner.SyncItems(); }
            public void Clear() { _items.Clear(); _owner.SyncItems(); }
            public bool Contains(object? i) => _items.Contains(i);
            public int IndexOf(object? i) => _items.IndexOf(i);
            public void Remove(object? i) { _items.Remove(i); _owner.SyncItems(); }
            public void RemoveAt(int i) { _items.RemoveAt(i); _owner.SyncItems(); }
            public IEnumerator GetEnumerator() => _items.GetEnumerator();
            bool IList.IsFixedSize => false; bool IList.IsReadOnly => false;
            object ICollection.SyncRoot => this; bool ICollection.IsSynchronized => false;
            int IList.Add(object? v) => Add(v);
            void IList.Clear() => Clear();
            bool IList.Contains(object? v) => Contains(v);
            int IList.IndexOf(object? v) => IndexOf(v);
            void IList.Insert(int i, object? v) { _items.Insert(i, v); _owner.SyncItems(); }
            void IList.Remove(object? v) => Remove(v);
            void IList.RemoveAt(int i) => RemoveAt(i);
            void ICollection.CopyTo(Array a, int i) => ((ICollection)_items).CopyTo(a, i);
        }
    }
    public class NumericUpDown : Control, ISupportInitialize {
        private readonly AvNumeric _n;
        public decimal Minimum { get => _n.Minimum; set => _n.Minimum = value; }
        public decimal Maximum { get => _n.Maximum; set => _n.Maximum = value; }
        public decimal Value { get => _n.Value ?? 0; set => _n.Value = value; }
        public decimal Increment { get => _n.Increment; set => _n.Increment = value; }
        public int DecimalPlaces { get => _n.FormatString?.Contains('.', StringComparison.Ordinal) == true ? 2 : 0; set => _n.FormatString = value > 0 ? "0." + new string('0', value) : "0"; }
        public bool ThousandsSeparator { get; set; }
        public bool Hexadecimal { get; set; }
        public LeftRightAlignment UpDownAlign { get; set; }
        public event EventHandler? ValueChanged;
        public NumericUpDown() : base(Make(out AvNumeric n)) {
            _n = n; _n.Minimum = 0; _n.Maximum = 100; _n.Increment = 1;
            _n.ValueChanged += (_, _) => ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        private static AvControl Make(out AvNumeric n) { n = new AvNumeric(); return n; }
        public void BeginInit() { }
        public void EndInit() { }
    }
    public enum LeftRightAlignment { Left = 0, Right = 1 }
    public class ProgressBar : Control {
        private readonly AvProgress _p;
        public int Minimum { get => (int)_p.Minimum; set => _p.Minimum = value; }
        public int Maximum { get => (int)_p.Maximum; set => _p.Maximum = value; }
        public int Value { get => (int)_p.Value; set => _p.Value = value; }
        public int Step { get; set; } = 10;
        public ProgressBarStyle Style { get; set; }
        public ProgressBar() : base(Make(out AvProgress p)) { _p = p; _p.Minimum = 0; _p.Maximum = 100; Height = 23; }
        private static AvControl Make(out AvProgress p) { p = new AvProgress(); return p; }
        public void PerformStep() => Value = Math.Min(Maximum, Value + Step);
    }
    public enum ProgressBarStyle { Blocks = 0, Continuous = 1, Marquee = 2 }
    public class PictureBox : Control {
        private readonly AvImage _img;
        public PictureBoxSizeMode SizeMode { get; set; }
        public Image? Image {
            get => _image;
            set { _image = value; ApplyImage(); }
        }
        private Image? _image;
        public PictureBox() : base(Make(out AvImage i)) { _img = i; Size = new Size(100, 50); }
        private static AvControl Make(out AvImage i) { i = new AvImage(); return i; }
        private void ApplyImage() {
            if (_image is Bitmap bmp) {
                using var ms = new IO.MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                _img.Source = new Avalonia.Media.Imaging.Bitmap(ms);
            } else _img.Source = null;
        }
    }
    public class TrackBar : Control {
        public int Minimum { get; set; }
        public int Maximum { get; set; } = 10;
        public int Value { get; set; }
        public int TickFrequency { get; set; } = 1;
        public TickStyle TickStyle { get; set; }
        public Orientation Orientation { get; set; }
        public event EventHandler? ValueChanged;
        public event EventHandler? Scroll;
    }
    public class VScrollBar : ScrollBar { public VScrollBar() { Width = 16; } }
    public class HScrollBar : ScrollBar { public HScrollBar() { Height = 16; } }
    public class ScrollBar : Control {
        public int Minimum { get; set; }
        public int Maximum { get; set; } = 100;
        public int Value { get; set; }
        public int LargeChange { get; set; } = 10;
        public int SmallChange { get; set; } = 1;
        public event ScrollEventHandler? Scroll;
        public event EventHandler? ValueChanged;
        protected void RaiseScroll() { Scroll?.Invoke(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, Value)); ValueChanged?.Invoke(this, EventArgs.Empty); }
    }
    public class LinkLabel : Label {
        public event EventHandler? LinkClicked;
        public Color LinkColor { get; set; } = Color.Blue;
    }
    public class DateTimePicker : Control {
        public DateTime Value { get; set; } = DateTime.Now;
        public event EventHandler? ValueChanged;
    }
    public class PropertyGrid : Control {
        public object? SelectedObject { get; set; }
    }
    public class StatusStrip : Control { public StatusStrip() { Height = 22; Dock = DockStyle.Bottom; } }
    public class MenuStrip : Control { public MenuStrip() { Height = 24; Dock = DockStyle.Top; } public ToolStripItemCollection Items { get; } = []; }
    public class ToolStrip : Control { public ToolStripItemCollection Items { get; } = []; }
}
