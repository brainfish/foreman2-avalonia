using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing {
    public enum ContentAlignment {
        TopLeft = 1, TopCenter = 2, TopRight = 4,
        MiddleLeft = 16, MiddleCenter = 32, MiddleRight = 64,
        BottomLeft = 256, BottomCenter = 512, BottomRight = 1024
    }

    [Flags]
    public enum FontStyle { Regular = 0, Bold = 1, Italic = 2, Underline = 4, Strikeout = 8 }

    public enum StringAlignment { Near = 0, Center = 1, Far = 2 }
    public enum StringTrimming { None = 0, Character = 1, Word = 2, EllipsisCharacter = 3, EllipsisWord = 4, EllipsisPath = 5 }
    [Flags]
    public enum StringFormatFlags { DirectionRightToLeft = 1, DirectionVertical = 2, FitBlackBox = 4, DisplayFormatControl = 32, NoFontFallback = 1024, MeasureTrailingSpaces = 2048, NoWrap = 4096, LineLimit = 8192, NoClip = 16384 }
    public enum GraphicsUnit { World = 0, Display = 1, Pixel = 2, Point = 3, Inch = 4, Document = 5, Millimeter = 6 }

    public static class ColorExtensions {
        public static float GetBrightness(this Color c) {
            float r = c.R / 255f, g = c.G / 255f, b = c.B / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            return (max + min) / 2f;
        }
    }

    public sealed class FontFamily : IDisposable {
        public static FontFamily GenericSansSerif { get; } = new("sans-serif");
        public static FontFamily GenericSerif { get; } = new("serif");
        public static FontFamily GenericMonospace { get; } = new("monospace");
        public string Name { get; }
        internal SKTypeface Typeface { get; }
        public FontFamily(string name) {
            Name = name;
            Typeface = Resolve(name, SKFontStyle.Normal);
        }
        internal FontFamily(string name, SKTypeface typeface) { Name = name; Typeface = typeface; }
        internal static SKTypeface Resolve(string name, SKFontStyle style) {
            SKTypeface? tf = SKTypeface.FromFamilyName(name, style);
            if (tf is null || tf.FamilyName == "sans-serif" && name != "sans-serif")
                tf = SKTypeface.FromFamilyName(null, style) ?? SKTypeface.Default;
            return tf;
        }
        public void Dispose() { }
    }

    public sealed class Font : IDisposable {
        public FontFamily FontFamily { get; }
        public float Size { get; }
        public FontStyle Style { get; }
        public GraphicsUnit Unit { get; set; } = GraphicsUnit.Point;
        public float SizeInPoints => Size;
        public bool Bold => (Style & FontStyle.Bold) != 0;
        public bool Italic => (Style & FontStyle.Italic) != 0;
        internal SKTypeface Typeface { get; }
        internal float SizePx => Size * 96f / 72f;

        public Font(Font prototype, FontStyle newStyle) : this(prototype.FontFamily.Name, prototype.Size, newStyle) { }
        public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular) { }
        public Font(FontFamily family, float emSize, FontStyle style) {
            FontFamily = family;
            Size = emSize;
            Style = style;
            Typeface = FontFamily.Resolve(family.Name, ToSkia(style));
        }
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit) : this(family, emSize, style) { Unit = unit; }
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(familyName, emSize, style) { Unit = unit; }
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte _) : this(familyName, emSize, style) { Unit = unit; }
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte _) : this(family, emSize, style) { Unit = unit; }
        public Font(string familyName, float emSize) : this(familyName, emSize, FontStyle.Regular) { }
        public Font(string familyName, float emSize, FontStyle style) {
            FontFamily = new FontFamily(familyName);
            Size = emSize;
            Style = style;
            Typeface = FontFamily.Resolve(familyName, ToSkia(style));
        }
        private static SKFontStyle ToSkia(FontStyle style) {
            SKFontStyleWeight w = (style & FontStyle.Bold) != 0 ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            SKFontStyleSlant s = (style & FontStyle.Italic) != 0 ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            return new SKFontStyle(w, SKFontStyleWidth.Normal, s);
        }
        public Font Clone() => new(FontFamily.Name, Size, Style);
        public void Dispose() { }
    }

    public static class SystemFonts {
        public static Font DefaultFont { get; } = new Font(FontFamily.GenericSansSerif, 9f);
        public static Font MessageBoxFont { get; } = DefaultFont;
    }

    public sealed class StringFormat : IDisposable {
        public StringAlignment Alignment { get; set; }
        public StringAlignment LineAlignment { get; set; }
        public StringTrimming Trimming { get; set; }
        public StringFormatFlags FormatFlags { get; set; }
        public static StringFormat GenericDefault => new();
        public static StringFormat GenericTypographic => new() { FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit };
        public StringFormat() { }
        public StringFormat(StringFormat format) {
            Alignment = format.Alignment;
            LineAlignment = format.LineAlignment;
            Trimming = format.Trimming;
            FormatFlags = format.FormatFlags;
        }
        public void Dispose() { }
    }

    public abstract class Brush : IDisposable {
        internal abstract SKPaint CreatePaint();
        public virtual void Dispose() { GC.SuppressFinalize(this); }
        public virtual object Clone() => this;
    }

    public sealed class SolidBrush : Brush {
        public Color Color { get; set; }
        public SolidBrush(Color color) { Color = color; }
        internal override SKPaint CreatePaint() => new() {
            Style = SKPaintStyle.Fill,
            Color = Graphics.ToSk(Color),
            IsAntialias = true
        };
    }

    public static class Brushes {
        public static Brush Black { get; } = new SolidBrush(Color.Black);
        public static Brush White { get; } = new SolidBrush(Color.White);
        public static Brush Transparent { get; } = new SolidBrush(Color.Transparent);
        public static Brush DarkGreen { get; } = new SolidBrush(Color.DarkGreen);
        public static Brush Gold { get; } = new SolidBrush(Color.Gold);
        public static Brush DarkBlue { get; } = new SolidBrush(Color.DarkBlue);
        public static Brush DarkRed { get; } = new SolidBrush(Color.DarkRed);
        public static Brush DarkOrange { get; } = new SolidBrush(Color.DarkOrange);
        public static Brush Goldenrod { get; } = new SolidBrush(Color.Goldenrod);
        public static Brush Orange { get; } = new SolidBrush(Color.Orange);
        public static Brush OrangeRed { get; } = new SolidBrush(Color.OrangeRed);
        public static Brush DimGray { get; } = new SolidBrush(Color.DimGray);
        public static Brush Gray { get; } = new SolidBrush(Color.Gray);
        public static Brush LightGray { get; } = new SolidBrush(Color.LightGray);
        public static Brush Red { get; } = new SolidBrush(Color.Red);
        public static Brush Blue { get; } = new SolidBrush(Color.Blue);
        public static Brush Green { get; } = new SolidBrush(Color.Green);
        public static Brush Yellow { get; } = new SolidBrush(Color.Yellow);
        public static Brush Coral { get; } = new SolidBrush(Color.Coral);
        public static Brush DarkGoldenrod { get; } = new SolidBrush(Color.DarkGoldenrod);
        public static Brush Crimson { get; } = new SolidBrush(Color.Crimson);
        public static Brush DarkGray { get; } = new SolidBrush(Color.DarkGray);
    }

    public sealed class Pen : IDisposable {
        public Brush Brush { get; set; }
        public float Width { get; set; }
        public Drawing2D.LineCap StartCap { get; set; }
        public Drawing2D.LineCap EndCap { get; set; }
        public Drawing2D.DashStyle DashStyle { get; set; }
        public Drawing2D.CustomLineCap? CustomEndCap { get; set; }
        public Drawing2D.CustomLineCap? CustomStartCap { get; set; }
        public Drawing2D.PenAlignment Alignment { get; set; }
        public Color Color => Brush is SolidBrush sb ? sb.Color : Color.Black;
        public Pen(Color color) : this(new SolidBrush(color), 1f) { }
        public Pen(Color color, float width) : this(new SolidBrush(color), width) { }
        public Pen(Brush brush) : this(brush, 1f) { }
        public Pen(Brush brush, float width) { Brush = brush; Width = width; }
        internal SKPaint CreatePaint() {
            SKPaint p = Brush.CreatePaint();
            p.Style = SKPaintStyle.Stroke;
            p.StrokeWidth = Math.Max(0.01f, Width);
            p.StrokeCap = EndCap == Drawing2D.LineCap.Round ? SKStrokeCap.Round : SKStrokeCap.Butt;
            p.StrokeJoin = SKStrokeJoin.Round;
            if (DashStyle == Drawing2D.DashStyle.Dash)
                p.PathEffect = SKPathEffect.CreateDash([6, 4], 0);
            else if (DashStyle == Drawing2D.DashStyle.Dot)
                p.PathEffect = SKPathEffect.CreateDash([2, 3], 0);
            else if (DashStyle == Drawing2D.DashStyle.DashDot)
                p.PathEffect = SKPathEffect.CreateDash([6, 3, 2, 3], 0);
            return p;
        }
        public void Dispose() { }
        public Pen Clone() => new(Brush, Width) { StartCap = StartCap, EndCap = EndCap, DashStyle = DashStyle };
    }

    public static class Pens {
        public static Pen Black { get; } = new(Color.Black);
        public static Pen White { get; } = new(Color.White);
        public static Pen Red { get; } = new(Color.Red);
        public static Pen Orange { get; } = new(Color.Orange);
        public static Pen Gray { get; } = new(Color.Gray);
        public static Pen DarkGray { get; } = new(Color.DarkGray);
        public static Pen LightGray { get; } = new(Color.LightGray);
        public static Pen Green { get; } = new(Color.Green);
    }

    public abstract class Image : IDisposable {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public Size Size => new(Width, Height);
        public Imaging.PixelFormat PixelFormat { get; protected set; } = Imaging.PixelFormat.Format32bppArgb;
        public float HorizontalResolution { get; protected set; } = 96;
        public float VerticalResolution { get; protected set; } = 96;
        public void SetResolution(float x, float y) { HorizontalResolution = x; VerticalResolution = y; }
        public abstract void Dispose();
        public void Save(string filename) => Save(filename, Imaging.ImageFormat.Png);
        public abstract void Save(string filename, Imaging.ImageFormat format);
        public abstract void Save(Stream stream, Imaging.ImageFormat format);
    }

    public sealed class Icon : IDisposable {
        public Icon(string fileName) { FileName = fileName; }
        public Icon(Stream stream) { using var ms = new MemoryStream(); stream.CopyTo(ms); Data = ms.ToArray(); }
        internal string? FileName { get; }
        internal byte[]? Data { get; }
        public void Dispose() { }
        public Bitmap ToBitmap() {
            if (FileName is not null)
                return new Bitmap(FileName);
            if (Data is not null)
                return new Bitmap(new MemoryStream(Data));
            return new Bitmap(16, 16);
        }
    }

    public sealed class Bitmap : Image {
        internal SKBitmap Skia { get; }
        private bool _own = true;
        public override int Width => Skia.Width;
        public override int Height => Skia.Height;

        public Bitmap(int width, int height) : this(width, height, Imaging.PixelFormat.Format32bppArgb) { }
        public Bitmap(int width, int height, Imaging.PixelFormat format) {
            PixelFormat = format;
            SKAlphaType alpha = format == Imaging.PixelFormat.Format32bppPArgb ? SKAlphaType.Premul : SKAlphaType.Unpremul;
            Skia = new SKBitmap(new SKImageInfo(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, alpha));
            Skia.Erase(SKColors.Transparent);
        }
        public Bitmap(string filename) {
            using SKBitmap? loaded = SKBitmap.Decode(filename);
            Skia = loaded is null ? new SKBitmap(32, 32, SKColorType.Bgra8888, SKAlphaType.Unpremul) : loaded.Copy() ?? loaded;
            PixelFormat = Imaging.PixelFormat.Format32bppArgb;
        }
        public Bitmap(Stream stream) {
            using SKBitmap? loaded = SKBitmap.Decode(stream);
            Skia = loaded is null ? new SKBitmap(32, 32, SKColorType.Bgra8888, SKAlphaType.Unpremul) : loaded.Copy() ?? loaded;
            PixelFormat = Imaging.PixelFormat.Format32bppArgb;
        }
        public Bitmap(Image original) : this(original.Width, original.Height, original.PixelFormat) {
            using var g = Graphics.FromImage(this);
            g.DrawImage(original, 0, 0, Width, Height);
        }
        internal Bitmap(SKBitmap skia, bool own) { Skia = skia; _own = own; PixelFormat = Imaging.PixelFormat.Format32bppArgb; }

        public static int GetPixelFormatSize(Imaging.PixelFormat pixelformat) => pixelformat switch {
            Imaging.PixelFormat.Format32bppArgb or Imaging.PixelFormat.Format32bppPArgb or Imaging.PixelFormat.Format32bppRgb => 32,
            Imaging.PixelFormat.Format24bppRgb => 24,
            _ => 32
        };

        public Imaging.BitmapData LockBits(Rectangle rect, Imaging.ImageLockMode _, Imaging.PixelFormat __) {
            Skia.NotifyPixelsChanged();
            return new Imaging.BitmapData {
                Scan0 = Skia.GetPixels(),
                Stride = Skia.RowBytes,
                Width = Skia.Width,
                Height = Skia.Height,
                PixelFormat = PixelFormat
            };
        }
        public void UnlockBits(Imaging.BitmapData _) { Skia.NotifyPixelsChanged(); }

        public Color GetPixel(int x, int y) {
            SKColor c = Skia.GetPixel(x, y);
            return Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
        }

        public override void Save(string filename, Imaging.ImageFormat format) {
            using var stream = File.Open(filename, FileMode.Create, FileAccess.Write);
            Save(stream, format);
        }
        public override void Save(Stream stream, Imaging.ImageFormat format) {
            using SKImage img = SKImage.FromBitmap(Skia);
            SKEncodedImageFormat enc = format.Equals(Imaging.ImageFormat.Jpeg) ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;
            using SKData data = img.Encode(enc, 100);
            data.SaveTo(stream);
        }
        public Bitmap Clone(Rectangle rect, Imaging.PixelFormat format) {
            var copy = new Bitmap(rect.Width, rect.Height, format);
            using var g = Graphics.FromImage(copy);
            g.DrawImage(this, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            return copy;
        }
        public override void Dispose() {
            if (_own)
                Skia.Dispose();
        }
    }

    public sealed class Region : IDisposable {
        internal SKPath Path { get; }
        public Region(Drawing2D.GraphicsPath path) { Path = new SKPath(path.Skia); }
        public Region(Rectangle rect) {
            Path = new SKPath();
            Path.AddRect(new SKRect(rect.X, rect.Y, rect.Right, rect.Bottom));
        }
        public void Dispose() => Path.Dispose();
    }

    public sealed class Graphics : IDisposable {
        internal SKCanvas Canvas { get; }
        private readonly bool _ownsCanvas;
        public Drawing2D.SmoothingMode SmoothingMode { get; set; }
        public Drawing2D.InterpolationMode InterpolationMode { get; set; }
        public Drawing2D.CompositingQuality CompositingQuality { get; set; }
        public Drawing2D.PixelOffsetMode PixelOffsetMode { get; set; }
        public Region? Clip { get; set; }

        internal Graphics(SKCanvas canvas, bool ownsCanvas = false) {
            Canvas = canvas;
            _ownsCanvas = ownsCanvas;
        }

        public static Graphics FromImage(Image image) {
            if (image is not Bitmap bmp)
                throw new ArgumentException("Expected Bitmap", nameof(image));
            return new Graphics(new SKCanvas(bmp.Skia), ownsCanvas: true);
        }

        public static SKColor ToSk(Color c) => new(c.R, c.G, c.B, c.A);
        private static SKRect ToRect(Rectangle r) => new(r.X, r.Y, r.Right, r.Bottom);
        private static SKRect ToRect(RectangleF r) => new(r.X, r.Y, r.Right, r.Bottom);

        public void Clear(Color color) => Canvas.Clear(ToSk(color));
        public void ResetTransform() => Canvas.ResetMatrix();
        public void TranslateTransform(float dx, float dy) => Canvas.Translate(dx, dy);
        public void ScaleTransform(float sx, float sy) => Canvas.Scale(sx, sy);
        public void RotateTransform(float angle) => Canvas.RotateDegrees(angle);
        public int Save() => Canvas.Save();
        public void Restore() => Canvas.Restore();
        public void Restore(int s) => Canvas.RestoreToCount(s);

        public void FillRectangle(Brush brush, Rectangle r) { using SKPaint p = brush.CreatePaint(); Canvas.DrawRect(ToRect(r), p); }
        public void FillRectangle(Brush brush, RectangleF r) { using SKPaint p = brush.CreatePaint(); Canvas.DrawRect(ToRect(r), p); }
        public void FillRectangle(Brush brush, int x, int y, int w, int h) => FillRectangle(brush, new Rectangle(x, y, w, h));
        public void FillRectangle(Brush brush, float x, float y, float w, float h) => FillRectangle(brush, new RectangleF(x, y, w, h));

        public void DrawRectangle(Pen pen, Rectangle r) { using SKPaint p = pen.CreatePaint(); Canvas.DrawRect(ToRect(r), p); }
        public void DrawRectangle(Pen pen, RectangleF r) { using SKPaint p = pen.CreatePaint(); Canvas.DrawRect(ToRect(r), p); }
        public void DrawRectangle(Pen pen, int x, int y, int w, int h) => DrawRectangle(pen, new Rectangle(x, y, w, h));
        public void DrawRectangle(Pen pen, float x, float y, float w, float h) => DrawRectangle(pen, new RectangleF(x, y, w, h));

        public void FillEllipse(Brush brush, Rectangle r) { using SKPaint p = brush.CreatePaint(); Canvas.DrawOval(ToRect(r), p); }
        public void FillEllipse(Brush brush, RectangleF r) { using SKPaint p = brush.CreatePaint(); Canvas.DrawOval(ToRect(r), p); }
        public void FillEllipse(Brush brush, int x, int y, int w, int h) => FillEllipse(brush, new Rectangle(x, y, w, h));
        public void DrawEllipse(Pen pen, Rectangle r) { using SKPaint p = pen.CreatePaint(); Canvas.DrawOval(ToRect(r), p); }
        public void DrawEllipse(Pen pen, RectangleF r) { using SKPaint p = pen.CreatePaint(); Canvas.DrawOval(ToRect(r), p); }
        public void DrawEllipse(Pen pen, int x, int y, int w, int h) => DrawEllipse(pen, new Rectangle(x, y, w, h));
        public void DrawEllipse(Pen pen, float x, float y, float w, float h) => DrawEllipse(pen, new RectangleF(x, y, w, h));

        public void DrawLine(Pen pen, Point a, Point b) => DrawLine(pen, a.X, a.Y, b.X, b.Y);
        public void DrawLine(Pen pen, PointF a, PointF b) => DrawLine(pen, a.X, a.Y, b.X, b.Y);
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2) { using SKPaint p = pen.CreatePaint(); Canvas.DrawLine(x1, y1, x2, y2, p); }
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) { using SKPaint p = pen.CreatePaint(); Canvas.DrawLine(x1, y1, x2, y2, p); }

        public void DrawLines(Pen pen, Point[] points) {
            if (points.Length < 2) return;
            using SKPaint p = pen.CreatePaint();
            using var path = new SKPath();
            path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Length; i++)
                path.LineTo(points[i].X, points[i].Y);
            Canvas.DrawPath(path, p);
        }

        public void DrawPolygon(Pen pen, Point[] points) {
            using SKPaint p = pen.CreatePaint();
            using SKPath path = Poly(points, true);
            Canvas.DrawPath(path, p);
        }
        public void FillPolygon(Brush brush, Point[] points) {
            using SKPaint p = brush.CreatePaint();
            using SKPath path = Poly(points, true);
            Canvas.DrawPath(path, p);
        }
        public void FillPolygon(Brush brush, PointF[] points) {
            using SKPaint p = brush.CreatePaint();
            using var path = new SKPath();
            path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Length; i++)
                path.LineTo(points[i].X, points[i].Y);
            path.Close();
            Canvas.DrawPath(path, p);
        }
        private static SKPath Poly(Point[] points, bool close) {
            var path = new SKPath();
            if (points.Length == 0) return path;
            path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i < points.Length; i++)
                path.LineTo(points[i].X, points[i].Y);
            if (close) path.Close();
            return path;
        }

        public void DrawBeziers(Pen pen, Point[] points) {
            if (points.Length < 4) return;
            using SKPaint p = pen.CreatePaint();
            using var path = new SKPath();
            path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i + 2 < points.Length; i += 3)
                path.CubicTo(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, points[i + 2].X, points[i + 2].Y);
            Canvas.DrawPath(path, p);
        }
        public void DrawBeziers(Pen pen, PointF[] points) {
            if (points.Length < 4) return;
            using SKPaint p = pen.CreatePaint();
            using var path = new SKPath();
            path.MoveTo(points[0].X, points[0].Y);
            for (int i = 1; i + 2 < points.Length; i += 3)
                path.CubicTo(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, points[i + 2].X, points[i + 2].Y);
            Canvas.DrawPath(path, p);
        }

        public void DrawPath(Pen pen, Drawing2D.GraphicsPath path) { using SKPaint p = pen.CreatePaint(); Canvas.DrawPath(path.Skia, p); }
        public void FillPath(Brush brush, Drawing2D.GraphicsPath path) { using SKPaint p = brush.CreatePaint(); Canvas.DrawPath(path.Skia, p); }

        public void DrawImage(Image image, Rectangle dest) => DrawImage(image, dest.X, dest.Y, dest.Width, dest.Height);
        public void DrawImage(Image image, RectangleF dest) => DrawImage(image, (int)dest.X, (int)dest.Y, (int)dest.Width, (int)dest.Height);
        public void DrawImage(Image image, int x, int y) {
            if (image is Bitmap bmp)
                Canvas.DrawBitmap(bmp.Skia, x, y);
        }
        public void DrawImage(Image image, int x, int y, int w, int h) {
            if (image is not Bitmap bmp) return;
            using SKPaint p = SamplingPaint();
            Canvas.DrawBitmap(bmp.Skia, new SKRect(x, y, x + w, y + h), p);
        }
        public void DrawImage(Image image, Rectangle dest, Rectangle src, GraphicsUnit _) {
            if (image is not Bitmap bmp) return;
            using SKPaint p = SamplingPaint();
            Canvas.DrawBitmap(bmp.Skia, new SKRect(src.X, src.Y, src.Right, src.Bottom), ToRect(dest), p);
        }
        public void DrawImage(Image image, Rectangle dest, int srcX, int srcY, int srcW, int srcH, GraphicsUnit unit, Imaging.ImageAttributes? attrs) {
            if (image is not Bitmap bmp) return;
            using SKPaint p = SamplingPaint();
            if (attrs?.ColorMatrix is { } m)
                p.ColorFilter = SKColorFilter.CreateColorMatrix(m.ToSkia());
            Canvas.DrawBitmap(bmp.Skia, new SKRect(srcX, srcY, srcX + srcW, srcY + srcH), ToRect(dest), p);
        }
        public void DrawImageUnscaled(Image image, int x, int y) => DrawImage(image, x, y);
        public void DrawImageUnscaled(Image image, Point pt) => DrawImage(image, pt.X, pt.Y);

        private SKPaint SamplingPaint() {
            SKFilterQuality q = InterpolationMode is Drawing2D.InterpolationMode.NearestNeighbor or Drawing2D.InterpolationMode.Low
                ? SKFilterQuality.None : SKFilterQuality.High;
            return new SKPaint { FilterQuality = q, IsAntialias = SmoothingMode != Drawing2D.SmoothingMode.None };
        }

        public void DrawString(string? s, Font font, Brush brush, RectangleF layout, StringFormat? format = null) {
            if (string.IsNullOrEmpty(s)) return;
            using SKPaint p = brush.CreatePaint();
            using SKFont skFont = MakeFont(font);
            format ??= StringFormat.GenericDefault;
            p.TextAlign = SKTextAlign.Left;
            p.IsAntialias = true;
            float x = layout.X;
            float y = layout.Y;
            float textW = p.MeasureText(s);
            float textH = skFont.Metrics.Descent - skFont.Metrics.Ascent;
            x = format.Alignment switch {
                StringAlignment.Center => layout.X + (layout.Width - textW) / 2f,
                StringAlignment.Far => layout.X + layout.Width - textW,
                _ => layout.X
            };
            y = format.LineAlignment switch {
                StringAlignment.Center => layout.Y + (layout.Height - textH) / 2f - skFont.Metrics.Ascent,
                StringAlignment.Far => layout.Y + layout.Height - skFont.Metrics.Descent,
                _ => layout.Y - skFont.Metrics.Ascent
            };
            bool wrap = (format.FormatFlags & StringFormatFlags.NoWrap) == 0 && layout.Width > 0;
            if (wrap && textW > layout.Width) {
                DrawWrapped(s, skFont, p, layout, format);
                return;
            }
            Canvas.DrawText(s, x, y, skFont, p);
        }
        public void DrawString(string? s, Font font, Brush brush, Rectangle layout, StringFormat? format = null) =>
            DrawString(s, font, brush, (RectangleF)layout, format);
        public void DrawString(string? s, Font font, Brush brush, Point pt) => DrawString(s, font, brush, new RectangleF(pt.X, pt.Y, 10000, 1000), new StringFormat());
        public void DrawString(string? s, Font font, Brush brush, PointF pt) => DrawString(s, font, brush, new RectangleF(pt.X, pt.Y, 10000, 1000), new StringFormat());
        public void DrawString(string? s, Font font, Brush brush, Point pt, StringFormat? format) =>
            DrawString(s, font, brush, new RectangleF(pt.X, pt.Y, 10000, 1000), format);
        public void DrawString(string? s, Font font, Brush brush, PointF pt, StringFormat? format) =>
            DrawString(s, font, brush, new RectangleF(pt.X, pt.Y, 10000, 1000), format);
        public void DrawString(string? s, Font font, Brush brush, float x, float y) => DrawString(s, font, brush, new PointF(x, y));

        private void DrawWrapped(string s, SKFont font, SKPaint paint, RectangleF layout, StringFormat format) {
            float lineH = font.Metrics.Descent - font.Metrics.Ascent;
            float y = layout.Y - font.Metrics.Ascent;
            var words = s.Split(' ');
            var line = "";
            foreach (string w in words) {
                string trial = string.IsNullOrEmpty(line) ? w : line + " " + w;
                if (paint.MeasureText(trial) > layout.Width && line.Length > 0) {
                    Canvas.DrawText(line, layout.X, y, font, paint);
                    y += lineH;
                    line = w;
                    if (y > layout.Bottom) return;
                } else line = trial;
            }
            if (line.Length > 0)
                Canvas.DrawText(line, layout.X, y, font, paint);
        }

        public SizeF MeasureString(string? text, Font font) {
            if (string.IsNullOrEmpty(text)) return SizeF.Empty;
            using SKPaint p = new() { IsAntialias = true };
            using SKFont skFont = MakeFont(font);
            float w = p.MeasureText(text);
            float h = skFont.Metrics.Descent - skFont.Metrics.Ascent;
            return new SizeF(w + 2, h + 2);
        }
        public SizeF MeasureString(string? text, Font font, int width) {
            SizeF raw = MeasureString(text, font);
            if (width <= 0 || raw.Width <= width) return raw;
            using SKFont skFont = MakeFont(font);
            float lineH = skFont.Metrics.Descent - skFont.Metrics.Ascent;
            int lines = Math.Max(1, (int)Math.Ceiling(raw.Width / width));
            return new SizeF(width, lineH * lines + 2);
        }
        public SizeF MeasureString(string? text, Font font, SizeF layoutArea) => MeasureString(text, font, (int)layoutArea.Width);

        private static SKFont MakeFont(Font font) => new(font.Typeface, font.SizePx);

        public void Dispose() {
            if (_ownsCanvas)
                Canvas.Dispose();
        }
    }
}

namespace System.Drawing.Drawing2D {
    public enum SmoothingMode { Default = 0, HighSpeed = 1, HighQuality = 2, None = 3, AntiAlias = 4 }
    public enum InterpolationMode { Default = 0, Low = 1, High = 2, Bilinear = 3, Bicubic = 4, NearestNeighbor = 5, HighQualityBilinear = 6, HighQualityBicubic = 7 }
    public enum CompositingQuality { Default = 0, HighSpeed = 1, HighQuality = 2, GammaCorrected = 3, AssumeLinear = 4 }
    public enum PixelOffsetMode { Default = 0, HighSpeed = 1, HighQuality = 2, None = 3, Half = 4 }
    public enum LineCap { Flat = 0, Square = 1, Round = 2, Triangle = 3, NoAnchor = 16, SquareAnchor = 17, RoundAnchor = 18, DiamondAnchor = 19, ArrowAnchor = 20, Custom = 255 }
    public enum DashStyle { Solid = 0, Dash = 1, Dot = 2, DashDot = 3, DashDotDot = 4, Custom = 5 }
    public enum WrapMode { Tile = 0, TileFlipX = 1, TileFlipY = 2, TileFlipXY = 3, Clamp = 4 }
    public enum FillMode { Alternate = 0, Winding = 1 }
    public enum PenAlignment { Center = 0, Inset = 1, Outset = 2, Left = 3, Right = 4 }
    public enum MatrixOrder { Prepend = 0, Append = 1 }
    public enum LineJoin { Miter = 0, Bevel = 1, Round = 2, MiterClipped = 3 }

    public sealed class GraphicsPath : IDisposable {
        internal SKPath Skia { get; } = new();
        public FillMode FillMode { get; set; }
        public void StartFigure() { }
        public void CloseFigure() => Skia.Close();
        public void AddLine(int x1, int y1, int x2, int y2) {
            if (Skia.IsEmpty) Skia.MoveTo(x1, y1);
            else Skia.LineTo(x1, y1);
            Skia.LineTo(x2, y2);
        }
        public void AddLine(Point a, Point b) => AddLine(a.X, a.Y, b.X, b.Y);
        public void AddArc(int x, int y, int w, int h, float startAngle, float sweepAngle) {
            var rect = new SKRect(x, y, x + w, y + h);
            if (Skia.IsEmpty)
                Skia.AddArc(rect, startAngle, sweepAngle);
            else
                Skia.ArcTo(rect, startAngle, sweepAngle, false);
        }
        public void AddRectangle(Rectangle r) => Skia.AddRect(new SKRect(r.X, r.Y, r.Right, r.Bottom));
        public void AddEllipse(Rectangle r) => Skia.AddOval(new SKRect(r.X, r.Y, r.Right, r.Bottom));
        public GraphicsPath Clone() {
            var copy = new GraphicsPath { FillMode = FillMode };
            copy.Skia.AddPath(Skia);
            return copy;
        }
        public void Dispose() => Skia.Dispose();
    }

    public class CustomLineCap : IDisposable {
        public CustomLineCap() { }
        public CustomLineCap(GraphicsPath? fillPath, GraphicsPath? strokePath) { }
        public void Dispose() { }
    }
    public class AdjustableArrowCap : CustomLineCap {
        public AdjustableArrowCap(float width, float height) { }
        public AdjustableArrowCap(float width, float height, bool filled) { }
    }
    public sealed class Matrix : IDisposable {
        internal SKMatrix Skia;
        public Matrix() { Skia = SKMatrix.Identity; }
        public void Dispose() { }
    }
}

namespace System.Drawing.Imaging {
    public enum PixelFormat {
        DontCare = 0,
        Format24bppRgb = 137224,
        Format32bppRgb = 139273,
        Format32bppArgb = 2498570,
        Format32bppPArgb = 925707,
    }
    public enum ImageLockMode { ReadOnly = 1, WriteOnly = 2, ReadWrite = 3 }
    public sealed class ImageFormat : IEquatable<ImageFormat> {
        public static ImageFormat Png { get; } = new("png");
        public static ImageFormat Jpeg { get; } = new("jpeg");
        public static ImageFormat Bmp { get; } = new("bmp");
        public static ImageFormat Gif { get; } = new("gif");
        private readonly string _n;
        private ImageFormat(string n) { _n = n; }
        public bool Equals(ImageFormat? other) => other is not null && _n == other._n;
        public override bool Equals(object? obj) => obj is ImageFormat f && Equals(f);
        public override int GetHashCode() => _n.GetHashCode(StringComparison.Ordinal);
    }
    public sealed class BitmapData {
        public IntPtr Scan0 { get; set; }
        public int Stride { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PixelFormat PixelFormat { get; set; }
    }
    public sealed class ColorMatrix {
        private readonly float[] _m;
        public ColorMatrix(float[][] matrix) {
            _m = new float[20];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 5; c++)
                    _m[c * 4 + r] = matrix[r][c];
        }
        public ColorMatrix() { _m = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0]; }
        internal float[] ToSkia() => _m;
    }
    public sealed class ImageAttributes : IDisposable {
        public ColorMatrix? ColorMatrix { get; private set; }
        public void SetColorMatrix(ColorMatrix matrix) => ColorMatrix = matrix;
        public void Dispose() { }
    }
}
