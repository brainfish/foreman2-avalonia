using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Foreman {
    internal class NativeMethods {
        public enum DwmWindowAttribute : uint {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20
        }

        public static int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute) => 0;

        public static void SelectAllItems(ListView list) {
            foreach (ListViewItem item in list.Items)
                item.Selected = true;
        }

        public static void DeselectAllItems(ListView list) {
            foreach (ListViewItem item in list.Items)
                item.Selected = false;
        }

        public static void SetItemState(ListView list, int itemIndex, int mask, int value) {
            bool selected = (value & 2) != 0;
            if (itemIndex < 0) {
                foreach (ListViewItem item in list.Items)
                    item.Selected = selected;
                return;
            }
            if (itemIndex >= 0 && itemIndex < list.Items.Count)
                list.Items[itemIndex].Selected = selected;
        }
    }
}
