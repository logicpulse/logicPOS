﻿using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class DocumentFinanceDialogPreview : BaseDialog
    {
        private readonly bool _debug = false;
        private Alignment _alignmentWindow;
        private readonly ArticleBag _articleBag;
        private readonly cfg_configurationcurrency _configurationCurrency;

        public DocumentFinanceDialogPreview(Window parentWindow, DialogFlags pDialogFlags, DocumentFinanceDialogPreviewMode pMode, ArticleBag pArticleBag, cfg_configurationcurrency pConfigurationCurrency)
            : base(parentWindow, pDialogFlags)
        {
            Size windowSize = new Size(700, 360);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_preview.png";

            //Parameters
            _articleBag = pArticleBag;
            _configurationCurrency = pConfigurationCurrency;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            //Init Local Vars
            string windowTitle;
            if (pMode == DocumentFinanceDialogPreviewMode.Preview)
            {
                windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_documentfinance_preview_totals_mode_preview");
                //ActionArea Buttons
                IconButtonWithText buttonOk = new IconButtonWithText(new ButtonSettings { Name = "touchButtonOk_DialogActionArea", Text = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_button_label_ok"), Font = FontSettings.ActionAreaButton, FontColor = ColorSettings.ActionAreaButtonFont, Icon = IconSettings.ActionOK, IconSize = SizeSettings.ActionAreaButtonIcon, ButtonSize = SizeSettings.ActionAreaButton });
                //ActionArea
                actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            }
            else
            {
                windowTitle = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_documentfinance_preview_totals_mode_confirm");
                //ActionArea Buttons
                IconButtonWithText buttonNo = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.No);
                IconButtonWithText buttonYes = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Yes);
                //ActionArea
                actionAreaButtons.Add(new ActionAreaButton(buttonYes, ResponseType.Yes));
                actionAreaButtons.Add(new ActionAreaButton(buttonNo, ResponseType.No));
            }
            windowTitle = string.Format("{0} [{1}]", windowTitle, _configurationCurrency.Acronym);

            InitUI();

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _alignmentWindow, actionAreaButtons);
        }

        private void InitUI()
        {
            bool debug = false;
            uint padding = 5;
            decimal exchangeRate = _configurationCurrency.ExchangeRate;
            List<Label> labelsTitle = new List<Label>();
            List<Label> labelsValue = new List<Label>();
            Pango.FontDescription fontDescriptionTitle = Pango.FontDescription.FromString("Bold 11");
            Pango.FontDescription fontDescriptionValue = Pango.FontDescription.FromString("11");

            //Titles: Table Tax
            Label labelTitleTaxDesignation = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"));
            Label labelTitleTaxValue = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_tax"));
            Label labelTitleTaxBase = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_tax_base"));
            Label labelTitleTaxTotal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totaltax_acronym"));
            //Titles: Table Totals
            Label labelTitleDiscountCustomer = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_discount_customer") + " (%)"); /* IN009206 */
            Label labelTitleTotalNet = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_totalnet"));
            Label labelTitleTotalGross = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totalgross"));
            Label labelTitleDiscountTotal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_total_discount")); /* IN009206 */
            /* IN009206 */
            //Label labelTitleDiscountPaymentConditions = new Label(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_discount_payment_conditions);
            Label labelTitleTotalTax = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totaltax")); /* IN009206 */
            Label labelTitleTotalFinal = new Label(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_totalfinal"));

            //Values: Table Totals
            /* IN009206 - Begin */
            Label labelValueTotalGross = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalGross * exchangeRate));
            Label labelValueDiscountCustomer = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.DiscountGlobal)) { WidthRequest = 100 };
            /* IN009206 */
            //Label labelValueDiscountPaymentConditions  = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(0.0m));
            Label labelValueDiscountTotal = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalDiscount * exchangeRate));
            Label labelValueTotalNet = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalNet * exchangeRate));
            Label labelValueTotalTax = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalTax * exchangeRate));
            Label labelValueTotalFinal = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalFinal * exchangeRate));
            //Add to Titles List 
            labelsTitle.Add(labelTitleTaxDesignation);
            labelsTitle.Add(labelTitleTaxValue);
            labelsTitle.Add(labelTitleTaxBase);
            labelsTitle.Add(labelTitleTaxTotal);

            labelsTitle.Add(labelTitleTotalGross);
            labelsValue.Add(labelValueTotalGross);

            labelsTitle.Add(labelTitleDiscountCustomer);
            labelsValue.Add(labelValueDiscountCustomer);

            /* IN009206 */
            //labelsTitle.Add(labelTitleDiscountPaymentConditions);
            //labelsValue.Add(labelValueDiscountPaymentConditions);

            labelsTitle.Add(labelTitleDiscountTotal);
            labelsValue.Add(labelValueDiscountTotal);

            labelsTitle.Add(labelTitleTotalNet);
            labelsValue.Add(labelValueTotalNet);

            labelsTitle.Add(labelTitleTotalTax);
            labelsValue.Add(labelValueTotalTax);
            /* IN009206 - End */

            labelsTitle.Add(labelTitleTotalFinal);
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
                //if (_debug) _logger.Debug(string.Format("{0}:{1}:{2}:{3}", item.Value.Designation, item.Key, item.Value.TotalBase, item.Value.Total));
                //Prepare Labels
                Label labelDesignation = new Label(item.Value.Designation);
                Label labelValue = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Key));
                Label labelTotalBase = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value.TotalBase * exchangeRate));
                Label labelTotal = new Label(LogicPOS.Utility.DataConversionUtils.DecimalToString(item.Value.Total * exchangeRate));
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
            tableTotal.Attach(labelTitleTotalGross, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding); /* IN009206 */
            tableTotal.Attach(labelValueTotalGross, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding); /* IN009206 */
            //Row1
            row++;
            tableTotal.Attach(labelTitleDiscountCustomer, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            tableTotal.Attach(labelValueDiscountCustomer, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row2
            row++;
            /* IN009206 */
            //tableTotal.Attach(labelTitleDiscountPaymentConditions, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //tableTotal.Attach(labelValueDiscountPaymentConditions, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row3
            /* IN009206 */
            row++;
            tableTotal.Attach(labelTitleDiscountTotal, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            tableTotal.Attach(labelValueDiscountTotal, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row4
            /* IN009206 */
            row++;
            tableTotal.Attach(labelTitleTotalNet, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            tableTotal.Attach(labelValueTotalNet, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row5
            row++;
            tableTotal.Attach(labelTitleTotalTax, 0, 1, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            tableTotal.Attach(labelValueTotalTax, 1, 2, row, row + 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //Row6
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
            eventboxTax.ModifyBg(StateType.Normal, Color.LightGray.ToGdkColor());
            eventboxTax.Add(tableTax);
            EventBox eventboxTotal = new EventBox() { VisibleWindow = debug };
            eventboxTotal.ModifyBg(StateType.Normal, Color.LightGray.ToGdkColor());
            eventboxTotal.Add(tableTotal);

            hbox.PackStart(eventboxTax, true, true, 0);
            hbox.PackStart(eventboxTotal, false, false, 0);

            EventBox eventbox = new EventBox() { VisibleWindow = debug, BorderWidth = padding };
            eventbox.ModifyBg(StateType.Normal, Color.LightCoral.ToGdkColor());
            eventbox.Add(hbox);

            _alignmentWindow = new Alignment(0.5f, 0.5f, 0.5f, 0.5f)
                {
                    eventbox
                };

        }
    }
}
