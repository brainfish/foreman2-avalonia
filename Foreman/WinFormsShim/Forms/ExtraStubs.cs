using System.Drawing;

namespace System.Windows.Forms {
    public enum SizeGripStyle { Auto = 0, Show = 1, Hide = 2 }
    [Flags]
    public enum TextFormatFlags {
        Default = 0, Left = 0, Top = 0, GlyphOverhangPadding = 0, HorizontalCenter = 1, Right = 2,
        VerticalCenter = 4, Bottom = 8, WordBreak = 16, SingleLine = 32, ExpandTabs = 64, NoClipping = 256,
        NoPrefix = 2048, LeftAndRightPadding = 0x40000000
    }
    public enum ColumnHeaderAutoResizeStyle { None = 0, HeaderSize = 1, ColumnContent = 2 }
    public class FlatButtonAppearance {
        public Color BorderColor { get; set; } = Color.Gray;
        public int BorderSize { get; set; } = 1;
        public Color MouseDownBackColor { get; set; }
        public Color MouseOverBackColor { get; set; }
        public Color CheckedBackColor { get; set; }
    }
    public static class CheckBoxRenderer {
        public static Size GetGlyphSize(Graphics g, VisualStyles.CheckBoxState state) => VisualStyles.CheckBoxRenderer.GetGlyphSize(g, state);
        public static void DrawCheckBox(Graphics g, Point p, VisualStyles.CheckBoxState state) => VisualStyles.CheckBoxRenderer.DrawCheckBox(g, p, state);
    }

    partial class Control {
        public int DeviceDpi => 96;
        public bool Focused => Native.IsFocused;
        public Form? ParentForm => FindForm();
        public bool ContainsFocus {
            get {
                if (Native.IsFocused) return true;
                foreach (Control c in Controls)
                    if (c.ContainsFocus) return true;
                return false;
            }
        }
        public Size PreferredSize => GetPreferredSize(Size.Empty);
    }
    partial class Button {
        public FlatButtonAppearance FlatAppearance { get; } = new();
    }
    partial class Form {
        public SizeGripStyle SizeGripStyle { get; set; }
    }
    partial class ListView {
        public bool LabelWrap { get; set; }
        public bool UseCompatibleStateImageBehavior { get; set; } = true;
        public bool Scrollable { get; set; } = true;
        public bool ShowItemToolTips { get; set; }
        public bool HoverSelection { get; set; }
        public bool HotTracking { get; set; }
        public bool AllowColumnReorder { get; set; }
        public bool AutoArrange { get; set; } = true;
        public ItemActivation Activation { get; set; }
    }
    public enum ItemActivation { Standard = 0, OneClick = 1, TwoClick = 2 }
    partial class TabControl {
        public bool Multiline { get; set; }
        public new Point Padding { get; set; }
        public Size ItemSize { get; set; }
        public ImageList? ImageList { get; set; }
    }
    partial class ColumnHeader {
        public int DisplayIndex { get; set; }
        public string Name { get; set; } = "";
    }
}
