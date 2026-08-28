using System.Drawing;

namespace System.Windows.Forms {
    internal static class WfLayout {
        public static void Arrange(Control owner, Rectangle client) {
            if (owner is SplitContainer split) {
                ArrangeSplit(split, client);
                return;
            }
            if (owner is TableLayoutPanel table) {
                ArrangeTable(table, client);
                return;
            }
            if (owner is FlowLayoutPanel flow) {
                ArrangeFlow(flow, client);
                return;
            }
            Rectangle remaining = new(client.X + owner.Padding.Left, client.Y + owner.Padding.Top,
                Math.Max(0, client.Width - owner.Padding.Horizontal), Math.Max(0, client.Height - owner.Padding.Vertical));
            var docked = new List<Control>();
            var rest = new List<Control>();
            foreach (Control c in owner.Controls) {
                if (!c.Visible) continue;
                if (c.Dock != DockStyle.None) docked.Add(c);
                else rest.Add(c);
            }
            for (int i = docked.Count - 1; i >= 0; i--) {
                Control c = docked[i];
                switch (c.Dock) {
                    case DockStyle.Fill:
                        c.SetBounds(remaining.X, remaining.Y, remaining.Width, remaining.Height);
                        remaining = Rectangle.Empty;
                        break;
                    case DockStyle.Top:
                        c.SetBounds(remaining.X, remaining.Y, remaining.Width, c.Height);
                        remaining.Y += c.Height;
                        remaining.Height = Math.Max(0, remaining.Height - c.Height);
                        break;
                    case DockStyle.Bottom:
                        c.SetBounds(remaining.X, remaining.Bottom - c.Height, remaining.Width, c.Height);
                        remaining.Height = Math.Max(0, remaining.Height - c.Height);
                        break;
                    case DockStyle.Left:
                        c.SetBounds(remaining.X, remaining.Y, c.Width, remaining.Height);
                        remaining.X += c.Width;
                        remaining.Width = Math.Max(0, remaining.Width - c.Width);
                        break;
                    case DockStyle.Right:
                        c.SetBounds(remaining.Right - c.Width, remaining.Y, c.Width, remaining.Height);
                        remaining.Width = Math.Max(0, remaining.Width - c.Width);
                        break;
                }
            }
            foreach (Control c in rest) {
                if (c.AutoSize) {
                    Size pref = c.GetPreferredSize(remaining.Size);
                    c.Size = pref;
                }
            }
        }

        private static void ArrangeSplit(SplitContainer split, Rectangle client) {
            int gap = Math.Max(1, split.SplitterWidth);
            if (split.Orientation == Orientation.Horizontal) {
                int dist = Math.Clamp(split.SplitterDistance, 0, Math.Max(0, client.Height - gap));
                split.Panel1.SetBounds(client.X, client.Y, client.Width, dist);
                split.Panel2.SetBounds(client.X, client.Y + dist + gap, client.Width, Math.Max(0, client.Height - dist - gap));
            } else {
                int dist = Math.Clamp(split.SplitterDistance, 0, Math.Max(0, client.Width - gap));
                split.Panel1.SetBounds(client.X, client.Y, dist, client.Height);
                split.Panel2.SetBounds(client.X + dist + gap, client.Y, Math.Max(0, client.Width - dist - gap), client.Height);
            }
        }

        private static void ArrangeTable(TableLayoutPanel table, Rectangle client) {
            int cols = Math.Max(1, table.ColumnCount);
            int rows = Math.Max(1, table.RowCount);
            while (table.ColumnStyles.Count < cols) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
            while (table.RowStyles.Count < rows) table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            int innerW = Math.Max(0, client.Width - table.Padding.Horizontal);
            int innerH = Math.Max(0, client.Height - table.Padding.Vertical);
            float[] colW = Distribute(table.ColumnStyles.Select(s => (Type: s.SizeType, Value: s.Width)).ToArray(), innerW, cols);
            float[] rowH = Distribute(table.RowStyles.Select(s => (Type: s.SizeType, Value: s.Height)).ToArray(), innerH, rows);

            // autosize rows from content
            for (int r = 0; r < rows; r++) {
                if (table.RowStyles[r].SizeType != SizeType.AutoSize) continue;
                float h = 0;
                foreach (Control c in table.Controls) {
                    var cell = table.GetPositionFromControl(c);
                    if (cell.Row != r || !c.Visible) continue;
                    Size pref = c.AutoSize ? c.GetPreferredSize(new Size((int)colW[Math.Clamp(cell.Column, 0, cols - 1)], 0)) : c.Size;
                    h = Math.Max(h, pref.Height + c.Margin.Vertical);
                }
                rowH[r] = Math.Max(h, 4);
            }

            float[] colX = new float[cols];
            float[] rowY = new float[rows];
            colX[0] = client.X + table.Padding.Left;
            rowY[0] = client.Y + table.Padding.Top;
            for (int i = 1; i < cols; i++) colX[i] = colX[i - 1] + colW[i - 1];
            for (int i = 1; i < rows; i++) rowY[i] = rowY[i - 1] + rowH[i - 1];

            foreach (Control c in table.Controls) {
                if (!c.Visible) continue;
                var pos = table.GetPositionFromControl(c);
                int col = Math.Clamp(pos.Column, 0, cols - 1);
                int row = Math.Clamp(pos.Row, 0, rows - 1);
                int cs = Math.Max(1, table.GetColumnSpan(c));
                int rs = Math.Max(1, table.GetRowSpan(c));
                float w = 0, h = 0;
                for (int i = 0; i < cs && col + i < cols; i++) w += colW[col + i];
                for (int i = 0; i < rs && row + i < rows; i++) h += rowH[row + i];
                var cell = new Rectangle((int)colX[col] + c.Margin.Left, (int)rowY[row] + c.Margin.Top,
                    Math.Max(0, (int)w - c.Margin.Horizontal), Math.Max(0, (int)h - c.Margin.Vertical));
                if (c.Dock == DockStyle.Fill || c.Anchor.HasFlag(AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom))
                    c.SetBounds(cell.X, cell.Y, cell.Width, cell.Height);
                else if (c.Dock == DockStyle.Fill)
                    c.SetBounds(cell.X, cell.Y, cell.Width, cell.Height);
                else {
                    int x = cell.X, y = cell.Y, cw = c.Width, ch = c.Height;
                    if (c.AutoSize) {
                        Size pref = c.GetPreferredSize(cell.Size);
                        cw = pref.Width; ch = pref.Height;
                    }
                    if (c.Dock == DockStyle.Fill) { x = cell.X; y = cell.Y; cw = cell.Width; ch = cell.Height; }
                    else {
                        // default: fill cell for designer-generated UIs
                        x = cell.X; y = cell.Y; cw = cell.Width; ch = Math.Max(ch, 0);
                        if (c.Dock != DockStyle.None)
                            ch = cell.Height;
                        else if (c is Label or CheckBox or RadioButton or Button or ComboBox or NumericUpDown or TextBox)
                            ch = c is Label && c.AutoSize ? ch : (c.Dock == DockStyle.None && cell.Height > 0 ? Math.Max(c.Height, Math.Min(c.Height + 8, cell.Height)) : c.Height);
                        if (c.Dock != DockStyle.None || table.GrowStyle != 0)
                            c.SetBounds(cell.X, cell.Y, cell.Width, cell.Height);
                        else
                            c.SetBounds(x, y, Math.Max(1, cw), Math.Max(1, ch));
                        continue;
                    }
                    c.SetBounds(x, y, cw, ch);
                }
            }
        }

        private static float[] Distribute((SizeType Type, float Value)[] styles, int total, int count) {
            var result = new float[count];
            float abs = 0, pct = 0;
            int auto = 0;
            for (int i = 0; i < count; i++) {
                var s = i < styles.Length ? styles[i] : (Type: SizeType.Percent, Value: 100f / count);
                if (s.Type == SizeType.Absolute) { result[i] = s.Value; abs += s.Value; }
                else if (s.Type == SizeType.Percent) pct += s.Value;
                else auto++;
            }
            float leftover = Math.Max(0, total - abs);
            float pctSpace = leftover;
            float autoSpace = leftover;
            if (pct > 0 && auto > 0) { pctSpace = leftover * 0.7f; autoSpace = leftover * 0.3f; }
            else if (pct > 0) autoSpace = 0;
            else pctSpace = 0;
            for (int i = 0; i < count; i++) {
                var s = i < styles.Length ? styles[i] : (Type: SizeType.Percent, Value: 100f / count);
                if (s.Type == SizeType.Percent)
                    result[i] = pct > 0 ? pctSpace * (s.Value / pct) : 0;
                else if (s.Type == SizeType.AutoSize)
                    result[i] = auto > 0 ? autoSpace / auto : 0;
            }
            return result;
        }

        private static void ArrangeFlow(FlowLayoutPanel flow, Rectangle client) {
            int x = client.X + flow.Padding.Left;
            int y = client.Y + flow.Padding.Top;
            int rowH = 0;
            int maxW = Math.Max(1, client.Width - flow.Padding.Horizontal);
            bool wrap = flow.WrapContents;
            bool vertical = flow.FlowDirection is FlowDirection.TopDown or FlowDirection.BottomUp;
            foreach (Control c in flow.Controls) {
                if (!c.Visible) continue;
                Size sz = c.AutoSize ? c.GetPreferredSize(new Size(maxW, 0)) : c.Size;
                if (c.AutoSize) c.Size = sz;
                if (vertical) {
                    c.SetBounds(x + c.Margin.Left, y + c.Margin.Top, sz.Width, sz.Height);
                    y += sz.Height + c.Margin.Vertical;
                } else {
                    if (wrap && x > client.X + flow.Padding.Left && x + sz.Width + c.Margin.Horizontal > client.Right) {
                        x = client.X + flow.Padding.Left;
                        y += rowH;
                        rowH = 0;
                    }
                    c.SetBounds(x + c.Margin.Left, y + c.Margin.Top, sz.Width, sz.Height);
                    x += sz.Width + c.Margin.Horizontal;
                    rowH = Math.Max(rowH, sz.Height + c.Margin.Vertical);
                }
            }
        }
    }

    public class ScrollableControl : Control {
        public ScrollableControl() { }
        internal ScrollableControl(Avalonia.Controls.Control native) : base(native) { }
        public bool HScroll { get; set; }
        public bool VScroll { get; set; }
        protected virtual Point AutoScrollOffset { get; set; }
    }
    public class ContainerControl : ScrollableControl {
        public ContainerControl() { }
        internal ContainerControl(Avalonia.Controls.Control native) : base(native) { }
        public Control? ActiveControl { get; set; }
        public bool Validate() => true;
        public bool ValidateChildren() => true;
    }
    public class Panel : ScrollableControl {
        public Panel() { }
        internal Panel(Avalonia.Controls.Control native) : base(native) { }
        public override Size GetPreferredSize(Size proposedSize) => base.GetPreferredSize(proposedSize);
    }
    public class GroupBox : Panel {
        public GroupBox() { Height = 80; }
        public override Size GetPreferredSize(Size proposedSize) {
            Size inner = base.GetPreferredSize(proposedSize);
            return new Size(inner.Width, inner.Height + 18);
        }
    }
    public class UserControl : ContainerControl {
        public UserControl() { }
        protected override void OnCreateControl() {
            base.OnCreateControl();
            OnLoad(EventArgs.Empty);
        }
    }
    public class TableLayoutPanel : Panel {
        public int ColumnCount { get; set; } = 1;
        public int RowCount { get; set; } = 1;
        public TableLayoutColumnStyleCollection ColumnStyles { get; } = [];
        public TableLayoutRowStyleCollection RowStyles { get; } = [];
        public TableLayoutPanelGrowStyle GrowStyle { get; set; }
        public void SetColumnSpan(Control c, int span) => _colSpan[c] = span;
        public void SetRowSpan(Control c, int span) => _rowSpan[c] = span;
        public int GetColumnSpan(Control c) => _colSpan.TryGetValue(c, out int s) ? s : 1;
        public int GetRowSpan(Control c) => _rowSpan.TryGetValue(c, out int s) ? s : 1;
        public void SetColumn(Control c, int col) => SetCell(c, col, GetRow(c));
        public void SetRow(Control c, int row) => SetCell(c, GetColumn(c), row);
        public int GetColumn(Control c) => GetPositionFromControl(c).Column;
        public int GetRow(Control c) => GetPositionFromControl(c).Row;
        public TableLayoutPanelCellPosition GetPositionFromControl(Control c) {
            if (Controls.TableCells.TryGetValue(c, out var p))
                return new TableLayoutPanelCellPosition(p.Col, p.Row);
            int i = Controls.IndexOf(c);
            if (ColumnCount <= 0) return new TableLayoutPanelCellPosition(0, 0);
            return new TableLayoutPanelCellPosition(i % ColumnCount, i / ColumnCount);
        }
        public Control? GetControlFromPosition(int col, int row) {
            foreach (Control c in Controls)
                if (GetColumn(c) == col && GetRow(c) == row) return c;
            return null;
        }
        private void SetCell(Control c, int col, int row) => Controls.TableCells[c] = (col, row);
        private readonly Dictionary<Control, int> _colSpan = [];
        private readonly Dictionary<Control, int> _rowSpan = [];
        public override Size GetPreferredSize(Size proposedSize) {
            int w = 0, h = 0;
            foreach (Control c in Controls) {
                if (!c.Visible) continue;
                Size s = c.AutoSize ? c.GetPreferredSize(Size.Empty) : c.Size;
                w = Math.Max(w, c.Left + s.Width);
                h = Math.Max(h, c.Top + s.Height);
            }
            if (w == 0) w = Width;
            if (h == 0) h = Height;
            return new Size(w + Padding.Horizontal, h + Padding.Vertical + 4);
        }
    }
    public class TableLayoutColumnStyleCollection : List<ColumnStyle> { }
    public class TableLayoutRowStyleCollection : List<RowStyle> { }
    public enum TableLayoutPanelGrowStyle { FixedSize = 0, AddRows = 1, AddColumns = 2 }
    public readonly struct TableLayoutPanelCellPosition {
        public int Column { get; }
        public int Row { get; }
        public TableLayoutPanelCellPosition(int column, int row) { Column = column; Row = row; }
    }
    public class FlowLayoutPanel : Panel {
        public FlowDirection FlowDirection { get; set; }
        public bool WrapContents { get; set; } = true;
        public override Size GetPreferredSize(Size proposedSize) {
            int x = Padding.Left, y = Padding.Top, rowH = 0, maxX = Padding.Left;
            int wrapW = proposedSize.Width > 0 ? proposedSize.Width : (Width > 0 ? Width : int.MaxValue);
            foreach (Control c in Controls) {
                if (!c.Visible) continue;
                Size s = c.AutoSize ? c.GetPreferredSize(Size.Empty) : c.Size;
                if (WrapContents && x > Padding.Left && x + s.Width + c.Margin.Horizontal > wrapW) {
                    x = Padding.Left; y += rowH; rowH = 0;
                }
                x += s.Width + c.Margin.Horizontal;
                maxX = Math.Max(maxX, x);
                rowH = Math.Max(rowH, s.Height + c.Margin.Vertical);
            }
            return new Size(maxX + Padding.Right, y + rowH + Padding.Bottom);
        }
    }
    public class SplitContainer : ContainerControl {
        public SplitterPanel Panel1 { get; }
        public SplitterPanel Panel2 { get; }
        public int SplitterDistance { get; set; } = 120;
        public int SplitterWidth { get; set; } = 4;
        public Orientation Orientation { get; set; }
        public FixedPanel FixedPanel { get; set; }
        public bool Panel1Collapsed { get; set; }
        public bool Panel2Collapsed { get; set; }
        public SplitContainer() {
            Panel1 = new SplitterPanel(this);
            Panel2 = new SplitterPanel(this);
            Controls.Add(Panel1);
            Controls.Add(Panel2);
        }
    }
    public class SplitterPanel : Panel {
        public SplitterPanel(SplitContainer _) { }
    }
    public partial class TabControl : ContainerControl {
        private readonly Avalonia.Controls.TabControl _av;
        public TabAlignment Alignment { get; set; }
        public TabSizeMode SizeMode { get; set; }
        public TabPageCollection TabPages { get; }
        public int SelectedIndex {
            get => _av.SelectedIndex;
            set {
                if (value >= 0 && value < TabPages.Count) {
                    _av.SelectedIndex = value;
                    SelectedTab = TabPages[value];
                }
            }
        }
        public TabPage? SelectedTab { get; set; }
        public event EventHandler? SelectedIndexChanged;
        public TabControl() : base(Make(out Avalonia.Controls.TabControl t)) {
            _av = t;
            TabPages = new TabPageCollection(this);
            _av.SelectionChanged += (_, _) => {
                int i = _av.SelectedIndex;
                if (i >= 0 && i < TabPages.Count)
                    SelectedTab = TabPages[i];
                RaiseSelected();
            };
        }
        private static Avalonia.Controls.Control Make(out Avalonia.Controls.TabControl t) {
            t = new Avalonia.Controls.TabControl();
            return t;
        }
        internal void AdoptTabPage(TabPage page) {
            TabPages.Adopt(page);
        }
        internal void RaiseSelected() => SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
    }
    public class TabPage : Panel {
        public string ToolTipText { get; set; } = "";
        public Image? Image { get; set; }
        public int ImageIndex { get; set; } = -1;
        internal Avalonia.Controls.TabItem? AvItem { get; set; }
        public TabPage() { }
        public TabPage(string text) { Text = text; }
        protected override void ApplyText() {
            if (AvItem is not null) AvItem.Header = Text;
        }
    }
    public class TabPageCollection : IEnumerable<TabPage> {
        private readonly TabControl _owner;
        private readonly List<TabPage> _pages = [];
        public TabPageCollection(TabControl owner) { _owner = owner; }
        public int Count => _pages.Count;
        public TabPage this[int i] => _pages[i];
        internal void Adopt(TabPage page) {
            if (_pages.Contains(page)) return;
            AddCore(page);
        }
        public void Add(TabPage page) {
            if (_pages.Contains(page)) return;
            AddCore(page);
        }
        private void AddCore(TabPage page) {
            _pages.Add(page);
            var item = new Avalonia.Controls.TabItem { Header = string.IsNullOrEmpty(page.Text) ? "Tab" : page.Text, Content = page.Native };
            page.AvItem = item;
            if (_owner.Native is Avalonia.Controls.TabControl av)
                av.Items.Add(item);
            if (_owner.SelectedTab is null) {
                _owner.SelectedTab = page;
                if (_owner.Native is Avalonia.Controls.TabControl av2)
                    av2.SelectedIndex = 0;
            }
        }
        public void Add(string text) => Add(new TabPage(text));
        public int IndexOf(TabPage? page) => page is null ? -1 : _pages.IndexOf(page);
        public IEnumerator<TabPage> GetEnumerator() => _pages.GetEnumerator();
        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
