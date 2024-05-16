using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosPricePickerDialog : PosBaseDialog
    {
        //Moçambique 5/4/21
        //Private Members
        //UI
        private readonly Fixed _fixedContent;

        public fin_article Article { get; set; }

        public static string PriceType;


        protected int _boxSpacing = 5;

        public PosPricePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, fin_article pArticle)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            Article = pArticle;

            //Init Local Vars
            string windowTitle = $"{CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_filepicker")}";
            _windowSize = new Size(300, 473);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_select_record.png";

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _fixedContent, actionAreaButtons);
        }

        [Obsolete]
        private void InitUI()
        {
            try
            {
                //Init Font Description
                Pango.FontDescription fontDescription = Pango.FontDescription.FromString(GeneralSettings.Settings["fontEntryBoxValue"]);
                //Init Picker
                VBox _vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Get PriceType Collection : Require Criteria to exclude SettingsApp.XpoOidUndefinedRecord, else we get a Price0 here
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) OR (Oid <> '{0}')", XPOSettings.XpoOidUndefinedRecord));
                XPCollection xpcConfigurationPriceType = new XPCollection(XPOSettings.Session, typeof(fin_configurationpricetype), criteriaOperator);

                xpcConfigurationPriceType.Sorting = XPOHelper.GetXPCollectionDefaultSortingCollection();
                //Define Max 5 Rows : 5 Prices
                int priceTypeCount = (xpcConfigurationPriceType.Count > 5) ? 5 : xpcConfigurationPriceType.Count;

                if (xpcConfigurationPriceType.Count > 0)
                {
                    XPOComboBox _priceComboBox = new XPOComboBox(XPOSettings.Session, typeof(fin_configurationpricetype), null, "Designation", criteriaOperator, null, 0);

                    _vbox.PackStart(_priceComboBox, false, false, 0);
                }

                //Loop and Render Columns
                //for (int i = 0; i < priceTypeCount; i++)
                //{
                //    //int priceTypeIndex = ((fin_configurationpricetype)xpcConfigurationPriceType[i]).EnumValue;

                //    ////FieldNames
                //    //string fieldNamePriceNormal = string.Format("Price{0}", priceTypeIndex);
                //    //string fieldNamePricePromotion = string.Format("Price{0}Promotion", priceTypeIndex);
                //    //string fieldNamePriceUsePromotionPrice = string.Format("Price{0}UsePromotionPrice", priceTypeIndex);
                //    ////PriceType
                //    //Label labelPriceType = new Label(((fin_configurationpricetype)xpcConfigurationPriceType[i]).Designation) { WidthRequest = col1width };
                //    //labelPriceType.SetAlignment(0.0F, 0.5F);

                //    ////Entrys
                //    //Entry entryPriceNormal = new Entry() { WidthRequest = col2width };
                //    //Entry entryPricePromotion = new Entry() { WidthRequest = col3width };
                //    //_crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPriceNormal, _dataSourceRow, fieldNamePriceNormal, SettingsApp.RegexDecimalGreaterEqualThanZero, true));
                //    //_crudWidgetList.Add(new GenericCRUDWidgetXPO(entryPricePromotion, _dataSourceRow, fieldNamePricePromotion, SettingsApp.RegexDecimalGreaterEqualThanZero, true));
                //    ////UsePromotion
                //    //CheckButton checkButtonUsePromotion = new CheckButton(string.Empty) { WidthRequest = col4width };
                //    //_crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonUsePromotion, _dataSourceRow, fieldNamePriceUsePromotionPrice));
                //    ////PackIt
                //    //hboxPrices = new HBox(false, _boxSpacing);
                //    //hboxPrices.PackStart(labelPriceType, true, true, 0);
                //    //hboxPrices.PackStart(entryPriceNormal, false, false, 0);
                //    //hboxPrices.PackStart(entryPricePromotion, false, false, 0);
                //    //hboxPrices.PackStart(checkButtonUsePromotion, false, false, 0);
                //    ////PackIt VBox
                //    //vboxPrices.PackStart(hboxPrices, false, false, 0);
                //}



                _fixedContent.Put(_vbox, 0, 0);
                //Events
                //_priceComboBox.Changed += _priceComboBox_Changed;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void _priceComboBox_Changed(object sender, EventArgs e)
        {

        }

        public static string RequestPriceTypeValue(Window pSourceWindow, DialogFlags pDialogFlags, fin_article pArticle)
        {
            PosPricePickerDialog dialog;

            dialog = new PosPricePickerDialog(pSourceWindow, pDialogFlags, pArticle);

            int response = dialog.Run();
            if (response == (int)ResponseType.Ok)
            {
                return PriceType;
            }

            dialog.Destroy();

            return "";

        }

    }
}
