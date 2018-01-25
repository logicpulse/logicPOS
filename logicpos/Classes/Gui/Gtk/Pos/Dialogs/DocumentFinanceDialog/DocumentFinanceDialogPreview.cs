using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    enum DocumentFinanceDialogPreviewMode {
        Preview, 
        Confirmation
    }

    class DocumentFinanceDialogPreview : PosBaseDialog
    {
        private bool _debug = false;
        private Alignment _alignmentWindow;
        private ArticleBag _articleBag;
        private CFG_ConfigurationCurrency _configurationCurrency;

        public DocumentFinanceDialogPreview(Window pSourceWindow, DialogFlags pDialogFlags, DocumentFinanceDialogPreviewMode pMode, ArticleBag pArticleBag, CFG_ConfigurationCurrency pConfigurationCurrency)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = string.Empty;
            Size windowSize = new Size(700, 360);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_preview.png");
            
            //Parameters
            _articleBag = pArticleBag;
            _configurationCurrency = pConfigurationCurrency;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            if (pMode == DocumentFinanceDialogPreviewMode.Preview)
            {
                windowTitle = Resx.window_title_dialog_documentfinance_preview_totals_mode_preview;
                //ActionArea Buttons
                TouchButtonIconWithText buttonOk = new TouchButtonIconWithText("touchButtonOk_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, Resx.global_button_label_ok, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, _fileActionOK, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
                //ActionArea
                actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            }
            else
            {
                windowTitle = Resx.window_title_dialog_documentfinance_preview_totals_mode_confirm;
                //ActionArea Buttons
                TouchButtonIconWithText buttonNo = ActionAreaButton.FactoryGetDialogButtonType(ActionAreaButton.PosBaseDialogButtonType.No);
                TouchButtonIconWithText buttonYes = ActionAreaButton.FactoryGetDialogButtonType(ActionAreaButton.PosBaseDialogButtonType.Yes);
                //ActionArea
                actionAreaButtons.Add(new ActionAreaButton(buttonYes, ResponseType.Yes));
                actionAreaButtons.Add(new ActionAreaButton(buttonNo, ResponseType.No));
            }
            windowTitle = string.Format("{0} [{1}]", windowTitle, _configurationCurrency.Acronym);

            InitUI();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _alignmentWindow, actionAreaButtons);
        }

        private void InitUI()
        {
            try
            {
                bool debug = false;
                uint padding = 5;
                decimal exchangeRate = _configurationCurrency.ExchangeRate;
                List<Label> labelsTitle = new List<Label>();
                List<Label> labelsValue = new List<Label>();
                Pango.FontDescription fontDescriptionTitle = Pango.FontDescription.FromString("Bold 11");
                Pango.FontDescription fontDescriptionValue = Pango.FontDescription.FromString("11");

                //Titles: Table Tax
                Label labelTitleTaxDesignation = new Label(Resx.global_designation);
                Label labelTitleTaxValue = new Label(Resx.global_tax);
                Label labelTitleTaxBase = new Label(Resx.global_total_tax_base);
                Label labelTitleTaxTotal = new Label(Resx.global_total);
                //Titles: Table Totals
                Label labelTitleDiscount = new Label(Resx.global_discount);
                Label labelTitleTotalNet = new Label(Resx.global_documentfinance_totalgross);
                Label labelTitleDiscountCustomer = new Label(Resx.global_documentfinance_discount_customer);
                Label labelTitleDiscountPaymentConditions = new Label(Resx.global_documentfinance_discount_payment_conditions);
                Label labelTitleTotalTax = new Label(Resx.global_documentfinance_totaltax);
                Label labelTitleTotalFinal = new Label(Resx.global_documentfinance_totalfinal);
                //Values: Table Totals
                Label labelValueDiscount = new Label(FrameworkUtils.DecimalToString(_articleBag.DiscountGlobal)) { WidthRequest = 100 };
                Label labelValueTotalNet = new Label(FrameworkUtils.DecimalToString(_articleBag.TotalNet * exchangeRate));
                Label labelValueDiscountCustomer = new Label(FrameworkUtils.DecimalToString(_articleBag.TotalDiscount * exchangeRate));
                Label labelValueDiscountPaymentConditions  = new Label(FrameworkUtils.DecimalToString(0.0m));
                Label labelValueTotalTax = new Label(FrameworkUtils.DecimalToString(_articleBag.TotalTax * exchangeRate));
                Label labelValueTotalFinal = new Label(FrameworkUtils.DecimalToString(_articleBag.TotalFinal * exchangeRate));
                //Add to Titles List 
                labelsTitle.Add(labelTitleTaxDesignation);
                labelsTitle.Add(labelTitleTaxValue);
                labelsTitle.Add(labelTitleTaxBase);
                labelsTitle.Add(labelTitleTaxTotal);
                labelsTitle.Add(labelTitleDiscount);
                labelsTitle.Add(labelTitleTotalNet);
                labelsTitle.Add(labelTitleDiscountCustomer);
                labelsTitle.Add(labelTitleDiscountPaymentConditions);
                labelsTitle.Add(labelTitleTotalTax);
                labelsTitle.Add(labelTitleTotalFinal);
                //Add Values to List
                labelsValue.Add(labelValueDiscount);
                labelsValue.Add(labelValueTotalNet);
                labelsValue.Add(labelValueDiscountCustomer);
                labelsValue.Add(labelValueDiscountPaymentConditions);
                labelsValue.Add(labelValueTotalTax);
                labelsValue.Add(labelValueTotalFinal);
                
                //Add Tax Table and Rows
                uint row = 0;
                Table tableTax = new Table(Convert.ToUInt16(_articleBag.TaxBag.Count), 4, false);
                //tableTax.WidthRequest = 380;
                //Row0
                tableTax.Attach(labelTitleTaxDesignation, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTax.Attach(labelTitleTaxValue, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTax.Attach(labelTitleTaxBase, 2, 3, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTax.Attach(labelTitleTaxTotal, 3, 4, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                foreach (var item in _articleBag.TaxBag.OrderBy(i => i.Key))
                {
                    row++;
                    if (_debug) _log.Debug(string.Format("{0}:{1}:{2}:{3}", item.Value.Designation, item.Key, item.Value.TotalBase, item.Value.Total));
                    //Prepare Labels
                    Label labelDesignation = new Label(item.Value.Designation);
                    Label labelValue = new Label(FrameworkUtils.DecimalToString(item.Key));
                    Label labelTotalBase = new Label(FrameworkUtils.DecimalToString(item.Value.TotalBase * exchangeRate));
                    Label labelTotal = new Label(FrameworkUtils.DecimalToString(item.Value.Total * exchangeRate));
                    //Add Row
                    tableTax.Attach(labelDesignation, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                    tableTax.Attach(labelValue, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                    tableTax.Attach(labelTotalBase, 2, 3, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                    tableTax.Attach(labelTotal, 3, 4, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                    //Add References to List
                    labelsValue.Add(labelDesignation);
                    labelsValue.Add(labelValue);
                    labelsValue.Add(labelTotalBase);
                    labelsValue.Add(labelTotal);
                }

                //Add Totals Table and Rows
                row = 0;
                Table tableTotal = new Table(6, 2, false);
                //tableTax.WidthRequest = 280;
                //Row0
                tableTotal.Attach(labelTitleDiscount, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueDiscount, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                //Row1
                row++;
                tableTotal.Attach(labelTitleTotalNet, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueTotalNet, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                //Row2
                row++;
                tableTotal.Attach(labelTitleDiscountCustomer, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueDiscountCustomer, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                //Row3
                row++;
                tableTotal.Attach(labelTitleDiscountPaymentConditions, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueDiscountPaymentConditions, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                //Row4
                row++;
                tableTotal.Attach(labelTitleTotalTax, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueTotalTax, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                //Row5
                row++;
                tableTotal.Attach(labelTitleTotalFinal, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
                tableTotal.Attach(labelValueTotalFinal, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

                //Format labels Title
                foreach (Label label in labelsTitle)
                {
                    label.ModifyFont(fontDescriptionTitle);
                    label.SetAlignment(1.0F, 0.5F);
                }
                //Format labels Value
                foreach (Label label in labelsValue)
                {
                    label.ModifyFont(fontDescriptionValue);
                    label.SetAlignment(1.0F, 0.5F);
                }

                //Final Pack
                HBox hbox = new HBox(false, 20);

                EventBox eventboxTax = new EventBox() { VisibleWindow = debug };
                eventboxTax.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Color.LightGray));
                eventboxTax.Add(tableTax);
                EventBox eventboxTotal = new EventBox() { VisibleWindow = debug };
                eventboxTotal.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Color.LightGray));
                eventboxTotal.Add(tableTotal);

                hbox.PackStart(eventboxTax, true, true, 0);
                hbox.PackStart(eventboxTotal, false, false, 0);

                EventBox eventbox = new EventBox() { VisibleWindow = debug, BorderWidth = padding };
                eventbox.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Color.LightCoral));
                eventbox.Add(hbox);

                _alignmentWindow = new Alignment(0.5f, 0.5f, 0.5f, 0.5f);
                _alignmentWindow.Add(eventbox);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
