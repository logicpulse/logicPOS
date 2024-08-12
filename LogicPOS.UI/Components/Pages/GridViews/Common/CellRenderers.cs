using Gtk;

namespace LogicPOS.UI.Components.GridViews
{
    public static class CellRenderers
    {
        public static readonly  Pango.FontDescription TitleFont = Pango.FontDescription.FromString(Settings.AppSettings.Instance.fontGenericTreeViewColumnTitle);
        public static readonly Pango.FontDescription CellFont = Pango.FontDescription.FromString(Settings.AppSettings.Instance.fontGenericTreeViewColumn);

        public static CellRenderer Title() => new CellRendererText
        {
            Alignment = Pango.Alignment.Left,
            FontDesc = TitleFont
        };

        public static CellRenderer Code() => new CellRendererText
        {
            Alignment = Pango.Alignment.Right,
            Xalign = 1.0F,
            ForegroundGdk = new Gdk.Color(255, 0, 0),
            FontDesc = CellFont
        };

        public static CellRenderer Text() => new CellRendererText
        {
            Alignment = Pango.Alignment.Left,
            FontDesc = CellFont
        };


    }
}
