using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Pickers;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogArticleSubFamily : EditDialog
    {
        public DialogArticleSubFamily(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_articlesubfamily"));
            SetSizeRequest(500, 445);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, true));

                //ButtonLabel
                Entry entryButtonLabel = new Entry();
                BOWidgetBox boxButtonLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_name"), entryButtonLabel);
                vboxTab1.PackStart(boxButtonLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxButtonLabel, Entity, "ButtonLabel", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //Family
                XPOComboBox xpoComboBoxFamily = new XPOComboBox(Entity.Session, typeof(fin_articlefamily), (Entity as fin_articlesubfamily).Family, "Designation", null);
                BOWidgetBox boxFamily = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_families"), xpoComboBoxFamily);
                vboxTab1.PackStart(boxFamily, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxFamily, Entity, "Family", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((fin_articlesubfamily)Entity).ButtonImage);
                fileChooserButtonImage.Filter = FilePicker.GetFileFilterImages();
                fileChooserButtonImage.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewButtonImage.Pixbuf = logicpos.Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewButtonImage.WidthRequest, fileChooserImagePreviewButtonImage.HeightRequest));
                BOWidgetBox boxfileChooserButtonImage = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_image"), fileChooserButtonImage);
                HBox hboxfileChooserAndimagePreviewButtonImage = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(boxfileChooserButtonImage, true, true, 0);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(fileChooserFrameImagePreviewButtonImage, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewButtonImage, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxfileChooserButtonImage, Entity, "ButtonImage", string.Empty, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //CommissionGroup
                XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(Entity.Session, typeof(pos_usercommissiongroup), (Entity as fin_articlesubfamily).CommissionGroup, "Designation", null);
                BOWidgetBox boxCommissionGroup = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_commission_group"), xpoComboBoxCommissionGroup);
                vboxTab2.PackStart(boxCommissionGroup, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, Entity, "CommissionGroup", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(Entity.Session, typeof(sys_configurationprinters), (Entity as fin_articlesubfamily).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_device_printer"), xpoComboBoxPrinter);
                vboxTab2.PackStart(boxPrinter, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrinter, Entity, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(Entity.Session, typeof(sys_configurationprinterstemplates), (Entity as fin_articlesubfamily).Template, "Designation", null);
                BOWidgetBox boxTemplate = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationPrintersTemplates"), xpoComboBoxTemplate);
                vboxTab2.PackStart(boxTemplate, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTemplate, Entity, "Template", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //DiscountGroup
                XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(Entity.Session, typeof(erp_customerdiscountgroup), (Entity as fin_articlesubfamily).DiscountGroup, "Designation", null);
                BOWidgetBox boxDiscountGroup = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_discount_group"), xpoComboBoxDiscountGroup);
                vboxTab2.PackStart(boxDiscountGroup, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, Entity, "DiscountGroup", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //VatOnTable
                XPOComboBox xpoComboBoxVatOnTable = new XPOComboBox(Entity.Session, typeof(fin_configurationvatrate), (Entity as fin_articlesubfamily).VatOnTable, "Designation", null);
                BOWidgetBox boxVatOnTable = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_vat_on_table"), xpoComboBoxVatOnTable);
                vboxTab2.PackStart(boxVatOnTable, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxVatOnTable, Entity, "VatOnTable", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //VatDirectSelling

                XPOComboBox xpoComboBoxVatDirectSelling = new XPOComboBox(Entity.Session, typeof(fin_configurationvatrate), (Entity as fin_articlesubfamily).VatDirectSelling, "Designation", null);
                BOWidgetBox boxVatDirectSelling = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_vat_direct_selling"), xpoComboBoxVatDirectSelling);
                vboxTab2.PackStart(boxVatDirectSelling, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxVatDirectSelling, Entity, "VatDirectSelling", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_edit_articlesubfamily_tab2_label")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
