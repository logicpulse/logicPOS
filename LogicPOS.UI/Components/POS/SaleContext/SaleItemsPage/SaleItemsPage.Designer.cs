using Gtk;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleItemsPage
    {
        private void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(SaleItem));

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Model;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }
        private EventBox CreateTotalPanel()
        {
            Gdk.Color bgColor = (Theme.EventBoxTotal.BackgroundColor as string).StringToGdkColor();
            Pango.FontDescription labelLabelTotalFont = Pango.FontDescription.FromString(Theme.EventBoxTotal.LabelLabelTotal.Font);
            Gdk.Color labelLabelTotalFontColor = (Theme.EventBoxTotal.LabelLabelTotal.FontColor as string).StringToGdkColor();
            float labelLabelTotalAlignmentX = Convert.ToSingle(Theme.EventBoxTotal.LabelLabelTotal.AlignmentX);
            Pango.FontDescription labelTotalFont = Pango.FontDescription.FromString(Theme.EventBoxTotal.LabelTotal.Font);
            Gdk.Color labelTotalFontColor = (Theme.EventBoxTotal.LabelTotal.FontColor as string).StringToGdkColor();
            float labelTotalAlignmentX = Convert.ToSingle(Theme.EventBoxTotal.LabelTotal.AlignmentX);

            int columnDesignationWidth = Convert.ToInt16(Theme.Columns.DesignationWidth);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);

            LabelTotal = new Label(GeneralUtils.GetResourceByName("global_total_ticket"));
            LabelTotal.ModifyFont(labelLabelTotalFont);
            LabelTotal.ModifyFg(StateType.Normal, labelLabelTotalFontColor);
            LabelTotal.SetAlignment(labelLabelTotalAlignmentX, 0.0F);

            LabelTotalValue = new Label();
            LabelTotalValue.ModifyFont(labelTotalFont);
            LabelTotalValue.ModifyFg(StateType.Normal, labelTotalFontColor);
            LabelTotalValue.SetAlignment(labelTotalAlignmentX, 0.0F);
            LabelTotalValue.Text = "0";

            HBox hboxTotal = new HBox(false, 4);
            hboxTotal.PackStart(LabelTotal, true, true, 5);
            hboxTotal.PackStart(LabelTotalValue, false, false, 5);

            EventBox eventBoxTotal = new EventBox() { BorderWidth = 0 };
            eventBoxTotal.ModifyBg(StateType.Normal, bgColor);
            eventBoxTotal.Add(hboxTotal);

            return eventBoxTotal;
        }
        public void SetOrderModeBackGround()
        {
            GridView.ModifyBase(StateType.Normal, AppSettings.Instance.ColorPosTicketListModeOrderMainBackground.ToGdkColor());
        }

        public void SetTicketModeBackGround()
        {
            GridView.ModifyBase(StateType.Normal, AppSettings.Instance.ColorPosTicketListModeTicketBackground.ToGdkColor());
        }
        private void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreatePriceColumn());
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(CreateDiscountColumn());
            GridView.AppendColumn(CreateVatColumn());
            GridView.AppendColumn(CreateTotalColumn());
        }

        private void Design()
        {
            VBox verticalLayout = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            verticalLayout.PackStart(scrolledWindow, true, true, 0);
            verticalLayout.PackStart(CreateTotalPanel(), false, false, 0);

            PackStart(verticalLayout);
        }
    }
}
