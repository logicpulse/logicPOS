namespace Patagames.Pdf.Net.Controls.WinForms
{
    internal struct RenderRect
    {
        public bool IsChecked { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Left { get { return X; } }
        public float Top { get { return Y; } }
        public float Right { get { return X + Width; } }
        public float Bottom { get { return Y + Height; } }
        public float Width { get; set; }
        public float Height { get; set; }

        public RenderRect(float x, float y, float width, float height, bool isChecked)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsChecked = isChecked;
        }
    }
}
