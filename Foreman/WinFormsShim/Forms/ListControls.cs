using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Avalonia.Controls;
using Image = System.Drawing.Image;
using AvControl = Avalonia.Controls.Control;
using AvListBox = Avalonia.Controls.ListBox;

namespace System.Windows.Forms {
    public partial class ListBox : Control {
        private readonly AvListBox _lb;
        public SelectionMode SelectionMode { get; set; } = SelectionMode.One;
        public DrawMode DrawMode { get; set; }
        public bool Sorted { get; set; }
        public bool IntegralHeight { get; set; } = true;
        public int ItemHeight { get; set; } = 15;
        public object? DataSource { get; set; }
        public string DisplayMember { get; set; } = "";
        public string ValueMember { get; set; } = "";
        public bool FormattingEnabled { get; set; }
        public const int NoMatches = -1;
        public ObjectCollection Items { get; }
        public SelectedIndexCollection SelectedIndices { get; }
        public SelectedObjectCollection SelectedItems { get; }
        public int SelectedIndex {
            get => _lb.SelectedIndex;
            set { _lb.SelectedIndex = value; SelectedIndexChanged?.Invoke(this, EventArgs.Empty); }
        }
        public object? SelectedItem {
            get => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null;
            set { int i = Items.IndexOf(value!); if (i >= 0) SelectedIndex = i; }
        }
        public event EventHandler? SelectedIndexChanged;
        public event EventHandler? SelectedValueChanged;
        public event DrawItemEventHandler? DrawItem;
        public ListBox() : base(Make(out AvListBox l)) {
            _lb = l;
            Items = new ObjectCollection(this);
            SelectedIndices = new SelectedIndexCollection(this);
            SelectedItems = new SelectedObjectCollection(this);
            _lb.SelectionChanged += (_, _) => {
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                SelectedValueChanged?.Invoke(this, EventArgs.Empty);
            };
        }
        private static AvControl Make(out AvListBox l) { l = new AvListBox(); return l; }
        public int IndexFromPoint(Point p) => IndexFromPoint(p.X, p.Y);
        public int IndexFromPoint(int x, int y) {
            if (Items.Count == 0) return NoMatches;
            int i = Math.Clamp(y / Math.Max(1, ItemHeight), 0, Items.Count - 1);
            return i;
        }
        internal static string FormatItem(object? item, string displayMember) {
            if (item is null) return "";
            if (string.IsNullOrEmpty(displayMember)) return item.ToString() ?? "";
            var prop = item.GetType().GetProperty(displayMember);
            return prop?.GetValue(item)?.ToString() ?? item.ToString() ?? "";
        }
        internal void Sync() {
            _lb.Items.Clear();
            foreach (var i in Items) _lb.Items.Add(FormatItem(i, DisplayMember));
        }
        public class ObjectCollection : IList {
            private readonly ListBox _o; private readonly List<object> _i = [];
            public ObjectCollection(ListBox o) { _o = o; }
            public int Count => _i.Count;
            public object this[int i] { get => _i[i]; set => _i[i] = value!; }
            public int Add(object item) { _i.Add(item); _o.Sync(); return _i.Count - 1; }
            public void AddRange(object[] items) { _i.AddRange(items); _o.Sync(); }
            public void Clear() { _i.Clear(); _o.Sync(); }
            public bool Contains(object item) => _i.Contains(item);
            public int IndexOf(object item) => _i.IndexOf(item);
            public void Remove(object item) { _i.Remove(item); _o.Sync(); }
            public void RemoveAt(int i) { _i.RemoveAt(i); _o.Sync(); }
            public IEnumerator GetEnumerator() => _i.GetEnumerator();
            bool IList.IsFixedSize => false; bool IList.IsReadOnly => false;
            object ICollection.SyncRoot => this; bool ICollection.IsSynchronized => false;
            int IList.Add(object? v) => Add(v!);
            void IList.Clear() => Clear(); bool IList.Contains(object? v) => v is not null && Contains(v);
            int IList.IndexOf(object? v) => v is null ? -1 : IndexOf(v);
            void IList.Insert(int i, object? v) { _i.Insert(i, v!); _o.Sync(); }
            void IList.Remove(object? v) { if (v is not null) Remove(v); }
            void IList.RemoveAt(int i) => RemoveAt(i);
            void ICollection.CopyTo(Array a, int i) => ((ICollection)_i).CopyTo(a, i);
        }
        public class SelectedIndexCollection : IEnumerable<int> {
            private readonly ListBox _o;
            public SelectedIndexCollection(ListBox o) { _o = o; }
            public int Count => _o.SelectedIndex >= 0 ? 1 : 0;
            public int this[int i] => _o.SelectedIndex;
            public IEnumerator<int> GetEnumerator() { if (_o.SelectedIndex >= 0) yield return _o.SelectedIndex; }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class SelectedObjectCollection : IEnumerable {
            private readonly ListBox _o;
            public SelectedObjectCollection(ListBox o) { _o = o; }
            public int Count => _o.SelectedItem is null ? 0 : 1;
            public object? this[int i] => _o.SelectedItem;
            public IEnumerator GetEnumerator() { if (_o.SelectedItem is not null) yield return _o.SelectedItem; }
        }
    }
    public class CheckedListBox : ListBox {
        private readonly HashSet<int> _checked = [];
        public bool CheckOnClick { get; set; }
        public CheckedIndexCollection CheckedIndices { get; }
        public CheckedItemCollection CheckedItems { get; }
        public event ItemCheckEventHandler? ItemCheck;
        public CheckedListBox() { CheckedIndices = new(this); CheckedItems = new(this); }
        public bool GetItemChecked(int index) => _checked.Contains(index);
        public void SetItemChecked(int index, bool value) {
            var old = GetItemChecked(index) ? CheckState.Checked : CheckState.Unchecked;
            var nv = value ? CheckState.Checked : CheckState.Unchecked;
            ItemCheck?.Invoke(this, new ItemCheckEventArgs(index, nv, old));
            if (value) _checked.Add(index); else _checked.Remove(index);
        }
        public CheckState GetItemCheckState(int index) => GetItemChecked(index) ? CheckState.Checked : CheckState.Unchecked;
        public void SetItemCheckState(int index, CheckState s) => SetItemChecked(index, s == CheckState.Checked);
        public class CheckedIndexCollection : IEnumerable<int> {
            private readonly CheckedListBox _o;
            public CheckedIndexCollection(CheckedListBox o) { _o = o; }
            public int Count => _o._checked.Count;
            public int this[int i] => _o._checked.ElementAt(i);
            public IEnumerator<int> GetEnumerator() => _o._checked.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class CheckedItemCollection : IEnumerable {
            private readonly CheckedListBox _o;
            public CheckedItemCollection(CheckedListBox o) { _o = o; }
            public int Count => _o._checked.Count;
            public object this[int i] => _o.Items[_o._checked.ElementAt(i)];
            public IEnumerator GetEnumerator() { foreach (int i in _o._checked) yield return _o.Items[i]; }
        }
    }
    public partial class ListView : Control {
        private readonly AvListBox _lb;
        public View View { get; set; } = View.Details;
        public bool CheckBoxes { get; set; }
        public bool FullRowSelect { get; set; }
        public bool GridLines { get; set; }
        public bool HideSelection { get; set; } = true;
        public bool MultiSelect { get; set; } = true;
        public bool OwnerDraw { get; set; }
        public ColumnHeaderStyle HeaderStyle { get; set; }
        public SortOrder Sorting { get; set; }
        public ImageList? SmallImageList { get; set; }
        public ImageList? LargeImageList { get; set; }
        public ColumnHeaderCollection Columns { get; }
        public ListViewItemCollection Items { get; }
        public SelectedListViewItemCollection SelectedItems { get; }
        public SelectedIndexCollection SelectedIndices { get; }
        public CheckedListViewItemCollection CheckedItems { get; }
        public ListViewItem? TopItem { get; set; }
        public Point AutoScrollOffset { get; set; }
        public event EventHandler? SelectedIndexChanged;
        public event ItemCheckedEventHandler? ItemChecked;
        public event ListViewItemSelectionChangedEventHandler? ItemSelectionChanged;
        public event EventHandler? ItemCheck;
        public ListView() : base(Make(out AvListBox l)) {
            _lb = l;
            Columns = new ColumnHeaderCollection(this);
            Items = new ListViewItemCollection(this);
            SelectedItems = new SelectedListViewItemCollection(this);
            SelectedIndices = new SelectedIndexCollection(this);
            CheckedItems = new CheckedListViewItemCollection(this);
            _lb.SelectionChanged += (_, _) => {
                int idx = _lb.SelectedIndex;
                for (int i = 0; i < Items.Count; i++)
                    Items[i].SetSelectedSilently(i == idx);
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            };
        }
        private static AvControl Make(out AvListBox l) { l = new AvListBox(); return l; }
        internal void RebuildVisual() {
            _lb.Items.Clear();
            foreach (var item in Items)
                _lb.Items.Add(item.Text);
        }
        internal void RaiseSelection(ListViewItem item, bool selected) {
            ItemSelectionChanged?.Invoke(this, new ListViewItemSelectionChangedEventArgs(item, Items.IndexOf(item), selected));
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
        internal void RaiseChecked(ListViewItem item) => ItemChecked?.Invoke(this, new ItemCheckedEventArgs(item));
        public void BeginUpdate() { }
        public void EndUpdate() { RebuildVisual(); Invalidate(); }
        public void EnsureVisible(int _) { }
        public void AutoResizeColumns(ColumnHeaderAutoResizeStyle _) { }
        public ListViewItem? GetItemAt(int x, int y) {
            int i = y / 22;
            return i >= 0 && i < Items.Count ? Items[i] : null;
        }
        public class ColumnHeaderCollection : IEnumerable<ColumnHeader> {
            private readonly List<ColumnHeader> _c = [];
            public ColumnHeaderCollection(ListView _) { }
            public int Count => _c.Count;
            public ColumnHeader this[int i] => _c[i];
            public int Add(ColumnHeader c) { _c.Add(c); return _c.Count - 1; }
            public ColumnHeader Add(string text) { var c = new ColumnHeader { Text = text }; _c.Add(c); return c; }
            public ColumnHeader Add(string text, int width) { var c = new ColumnHeader { Text = text, Width = width }; _c.Add(c); return c; }
            public void AddRange(ColumnHeader[] cols) { _c.AddRange(cols); }
            public void Clear() => _c.Clear();
            public IEnumerator<ColumnHeader> GetEnumerator() => _c.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class ListViewItemCollection : IEnumerable<ListViewItem> {
            private readonly ListView _o; private readonly List<ListViewItem> _i = [];
            public ListViewItemCollection(ListView o) { _o = o; }
            public int Count => _i.Count;
            public ListViewItem this[int i] => _i[i];
            public ListViewItem Add(ListViewItem item) { item.ListView = _o; _i.Add(item); _o.RebuildVisual(); return item; }
            public ListViewItem Add(string text) => Add(new ListViewItem(text));
            public void AddRange(ListViewItem[] items) { foreach (var i in items) Add(i); }
            public void Clear() { _i.Clear(); }
            public int IndexOf(ListViewItem item) => _i.IndexOf(item);
            public void Remove(ListViewItem item) => _i.Remove(item);
            public void RemoveAt(int i) => _i.RemoveAt(i);
            public IEnumerator<ListViewItem> GetEnumerator() => _i.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class SelectedListViewItemCollection : IEnumerable<ListViewItem> {
            private readonly ListView _o;
            public SelectedListViewItemCollection(ListView o) { _o = o; }
            public int Count => _o.Items.Count(i => i.Selected);
            public ListViewItem this[int i] => _o.Items.Where(x => x.Selected).ElementAt(i);
            public void Clear() { foreach (var i in _o.Items) i.Selected = false; }
            public IEnumerator<ListViewItem> GetEnumerator() => _o.Items.Where(i => i.Selected).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class SelectedIndexCollection : IEnumerable<int> {
            private readonly ListView _o;
            public SelectedIndexCollection(ListView o) { _o = o; }
            public int Count => _o.Items.Count(i => i.Selected);
            public int this[int i] => Enumerable.Range(0, _o.Items.Count).Where(n => _o.Items[n].Selected).ElementAt(i);
            public void Clear() { foreach (var item in _o.Items) item.Selected = false; }
            public void Add(int index) { if (index >= 0 && index < _o.Items.Count) _o.Items[index].Selected = true; }
            public IEnumerator<int> GetEnumerator() { for (int i = 0; i < _o.Items.Count; i++) if (_o.Items[i].Selected) yield return i; }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class CheckedListViewItemCollection : IEnumerable<ListViewItem> {
            private readonly ListView _o;
            public CheckedListViewItemCollection(ListView o) { _o = o; }
            public int Count => _o.Items.Count(i => i.Checked);
            public ListViewItem this[int i] => _o.Items.Where(x => x.Checked).ElementAt(i);
            public IEnumerator<ListViewItem> GetEnumerator() => _o.Items.Where(i => i.Checked).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
    public partial class ColumnHeader {
        public string Text { get; set; } = "";
        public int Width { get; set; } = 60;
        public HorizontalAlignment TextAlign { get; set; }
        public ColumnHeader() { }
        public ColumnHeader(string text) { Text = text; }
    }
    public class ListViewItem {
        internal ListView? ListView { get; set; }
        public string Name { get; set; } = "";
        public string Text { get => SubItems.Count > 0 ? SubItems[0].Text : ""; set { if (SubItems.Count == 0) SubItems.Add(new ListViewSubItem(this, value)); else SubItems[0].Text = value; } }
        public bool Checked { get => _checked; set { _checked = value; ListView?.RaiseChecked(this); } }
        private bool _checked;
        public bool Selected { get => _selected; set { _selected = value; ListView?.RaiseSelection(this, value); } }
        internal void SetSelectedSilently(bool value) => _selected = value;
        private bool _selected;
        public int ImageIndex { get; set; } = -1;
        public object? Tag { get; set; }
        public Color BackColor { get; set; } = Color.White;
        public Color ForeColor { get; set; } = Color.Black;
        public Font? Font { get; set; }
        public string ToolTipText { get; set; } = "";
        public Rectangle Bounds {
            get {
                int i = Index;
                int h = 22;
                return new Rectangle(0, Math.Max(0, i) * h, ListView?.Width ?? 100, h);
            }
        }
        public int Index => ListView?.Items.IndexOf(this) ?? -1;
        public ListViewSubItemCollection SubItems { get; }
        public ListViewGroup? Group { get; set; }
        public ListViewItem() { SubItems = new ListViewSubItemCollection(this); SubItems.Add(new ListViewSubItem(this, "")); }
        public ListViewItem(string text) : this() { Text = text; }
        public ListViewItem(string[] items) : this() {
            SubItems.Clear();
            foreach (var s in items) SubItems.Add(new ListViewSubItem(this, s));
        }
        public class ListViewSubItem {
            public object? Tag { get; set; }
            public string Text { get; set; } = "";
            public Color BackColor { get; set; }
            public Color ForeColor { get; set; }
            public Font? Font { get; set; }
            public ListViewSubItem() { }
            public ListViewSubItem(ListViewItem _, string text) { Text = text; }
        }
        public class ListViewSubItemCollection : IEnumerable<ListViewSubItem> {
            private readonly List<ListViewSubItem> _i = [];
            public ListViewSubItemCollection(ListViewItem _) { }
            public int Count => _i.Count;
            public ListViewSubItem this[int i] => _i[i];
            public ListViewSubItem Add(ListViewSubItem s) { _i.Add(s); return s; }
            public ListViewSubItem Add(string t) { var s = new ListViewSubItem { Text = t }; _i.Add(s); return s; }
            public void Clear() => _i.Clear();
            public IEnumerator<ListViewSubItem> GetEnumerator() => _i.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
    public class ListViewGroup { public string Header { get; set; } = ""; }
    public class ImageList : IDisposable, IComponent {
        public ColorDepth ColorDepth { get; set; } = ColorDepth.Depth32Bit;
        public Size ImageSize { get; set; } = new(16, 16);
        public Color TransparentColor { get; set; } = Color.Transparent;
        public ImageCollection Images { get; } = [];
        public ISite? Site { get; set; }
        public event EventHandler? Disposed;
        public ImageList() { }
        public ImageList(IContainer _) { }
        public void Dispose() { Disposed?.Invoke(this, EventArgs.Empty); GC.SuppressFinalize(this); }
        public class ImageCollection : IEnumerable<Image> {
            private readonly List<Image> _i = [];
            public int Count => _i.Count;
            public Image this[int i] => _i[i];
            public int Add(Image img) { _i.Add(img); return _i.Count - 1; }
            public void Add(string _, Image img) => Add(img);
            public void Clear() => _i.Clear();
            public IEnumerator<Image> GetEnumerator() => _i.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
