using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms {
    public class AccessibleObject { }
    public class RetrieveVirtualItemEventArgs : EventArgs {
        public int ItemIndex { get; }
        public ListViewItem? Item { get; set; }
        public RetrieveVirtualItemEventArgs(int itemIndex) { ItemIndex = itemIndex; }
    }
    public delegate void RetrieveVirtualItemEventHandler(object? sender, RetrieveVirtualItemEventArgs e);
    public class ColumnClickEventArgs : EventArgs {
        public int Column { get; }
        public ColumnClickEventArgs(int column) { Column = column; }
    }
    public delegate void ColumnClickEventHandler(object? sender, ColumnClickEventArgs e);
    public class CacheVirtualItemsEventArgs : EventArgs {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public CacheVirtualItemsEventArgs(int start, int end) { StartIndex = start; EndIndex = end; }
    }
    public class SearchForVirtualItemEventArgs : EventArgs {
        public ListViewItem? Item { get; set; }
    }
}

namespace System.Windows.Forms {
    partial class Control {
        protected virtual bool ShowFocusCues => true;
        protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData) => false;
        protected virtual bool IsInputKey(Keys keyData) => false;
    }
    partial class ListBox {
        protected virtual void OnDrawItem(DrawItemEventArgs e) => DrawItem?.Invoke(this, e);
    }
    partial class ListView {
        public bool VirtualMode { get; set; }
        public int VirtualListSize { get; set; }
        public event RetrieveVirtualItemEventHandler? RetrieveVirtualItem;
        public event ColumnClickEventHandler? ColumnClick;
        public ListViewItem FindItemWithText(string _) => Items.Count > 0 ? Items[0] : new ListViewItem();
        internal ListViewItem GetVirtualItem(int index) {
            var args = new RetrieveVirtualItemEventArgs(index);
            RetrieveVirtualItem?.Invoke(this, args);
            return args.Item ?? new ListViewItem();
        }
    }
}
