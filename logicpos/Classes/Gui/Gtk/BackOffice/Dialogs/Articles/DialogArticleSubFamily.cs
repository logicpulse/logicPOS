using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogArticleSubFamily : BOBaseDialog
    {
        public DialogArticleSubFamily(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_articlesubfamily"));
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
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumeric, true));

                //ButtonLabel
                Entry entryButtonLabel = new Entry();
                BOWidgetBox boxButtonLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_name"), entryButtonLabel);
                vboxTab1.PackStart(boxButtonLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxButtonLabel, _dataSourceRow, "ButtonLabel", SettingsApp.RegexAlfaNumericExtended, false));

                //Family
                XPOComboBox xpoComboBoxFamily = new XPOComboBox(DataSourceRow.Session, typeof(fin_articlefamily), (DataSourceRow as fin_articlesubfamily).Family, "Designation", null);
                BOWidgetBox boxFamily = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_families"), xpoComboBoxFamily);
                vboxTab1.PackStart(boxFamily, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFamily, DataSourceRow, "Family", SettingsApp.RegexGuid, true));

                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((fin_articlesubfamily)DataSourceRow).ButtonImage);
                fileChooserButtonImage.Filter = Utils.GetFileFilterImages();
                fileChooserButtonImage.SelectionChanged += (sender, eventArgs) => fileChooserImagePreviewButtonImage.Pixbuf = Utils.ResizeAndCropFileToPixBuf((sender as FileChooserButton).Filename, new System.Drawing.Size(fileChooserImagePreviewButtonImage.WidthRequest, fileChooserImagePreviewButtonImage.HeightRequest));
                BOWidgetBox boxfileChooserButtonImage = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_image"), fileChooserButtonImage);
                HBox hboxfileChooserAndimagePreviewButtonImage = new HBox(false, _boxSpacing);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(boxfileChooserButtonImage, true, true, 0);
                hboxfileChooserAndimagePreviewButtonImage.PackStart(fileChooserFrameImagePreviewButtonImage, false, false, 0);
                vboxTab1.PackStart(hboxfileChooserAndimagePreviewButtonImage, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxfileChooserButtonImage, _dataSourceRow, "ButtonImage", string.Empty, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = SettingsApp.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //CommissionGroup
                XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(DataSourceRow.Session, typeof(pos_usercommissiongroup), (DataSourceRow as fin_articlesubfamily).CommissionGroup, "Designation", null);
                BOWidgetBox boxCommissionGroup = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_commission_group"), xpoComboBoxCommissionGroup);
                vboxTab2.PackStart(boxCommissionGroup, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, DataSourceRow, "CommissionGroup", SettingsApp.RegexGuid, false));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as fin_articlesubfamily).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_device_printer"), xpoComboBoxPrinter);
                vboxTab2.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, false));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as fin_articlesubfamily).Template, "Designation", null);
                BOWidgetBox boxTemplate = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationPrintersTemplates"), xpoComboBoxTemplate);
                vboxTab2.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", SettingsApp.RegexGuid, false));

                //DiscountGroup
                XPOComboBox xpoComboBoxDiscountGroup = new XPOComboBox(DataSourceRow.Session, typeof(erp_customerdiscountgroup), (DataSourceRow as fin_articlesubfamily).DiscountGroup, "Designation", null);
                BOWidgetBox boxDiscountGroup = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_discount_group"), xpoComboBoxDiscountGroup);
                vboxTab2.PackStart(boxDiscountGroup, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDiscountGroup, DataSourceRow, "DiscountGroup", SettingsApp.RegexGuid, false));

                //VatOnTable
                XPOComboBox xpoComboBoxVatOnTable = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_articlesubfamily).VatOnTable, "Designation", null);
                BOWidgetBox boxVatOnTable = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_on_table"), xpoComboBoxVatOnTable);
                vboxTab2.PackStart(boxVatOnTable, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatOnTable, DataSourceRow, "VatOnTable", SettingsApp.RegexGuid, false));

                //VatDirectSelling

                XPOComboBox xpoComboBoxVatDirectSelling = new XPOComboBox(DataSourceRow.Session, typeof(fin_configurationvatrate), (DataSourceRow as fin_articlesubfamily).VatDirectSelling, "Designation", null);
                BOWidgetBox boxVatDirectSelling = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_direct_selling"), xpoComboBoxVatDirectSelling);
                vboxTab2.PackStart(boxVatDirectSelling, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxVatDirectSelling, DataSourceRow, "VatDirectSelling", SettingsApp.RegexGuid, false));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_edit_articlesubfamily_tab2_label")));
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
