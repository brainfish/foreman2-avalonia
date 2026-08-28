using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Foreman.Controls {
    //pretty much this \/ . Didnt want to bother with something more complicated.
    //https://stackoverflow.com/questions/14726146/scrolling-list-view-when-another-list-view-is-scrolled
    //NOTE: using the 'sendmessage' approached failed, so had to switch to a 'set-top-index' approach
    internal class SyncListView : FFListView {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SyncListView? Buddy { get; set; }

        [DefaultValue(true)]
        public bool SyncScrolling { get; set; }

        [DefaultValue(true)]
        public bool SyncSelection { get; set; }

        private static bool scrolling;   // In case buddy tries to scroll us

        public SyncListView() {
            SyncScrolling = true;
            SyncSelection = true;

            this.ItemSelectionChanged += SyncListView_ItemSelectionChanged;
        }

        private void SyncListView_ItemSelectionChanged(object? sender, ListViewItemSelectionChangedEventArgs e) {
            if (SyncSelection && Buddy?.IsHandleCreated is true && Buddy.Items[e.ItemIndex].Selected != e.IsSelected)
                Buddy.Items[e.ItemIndex].Selected = e.IsSelected;
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if (SyncScrolling && !scrolling && Buddy != null && Buddy.IsHandleCreated && Buddy.Items.Count > 0) {
                scrolling = true;
                int index = TopItem?.Index ?? 0;
                if (index >= 0 && index < Buddy.Items.Count)
                    Buddy.TopItem = Buddy.Items[index];
                scrolling = false;
            }
        }
    }

    internal class FFListView : ListView {
        public FFListView() : base() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
