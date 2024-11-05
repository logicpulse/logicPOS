using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Pickers;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationPlace : EditDialog
    {
        public DialogConfigurationPlace(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_configurationplacetable"));

            SetSizeRequest(500, 450);
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
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //PriceType
                XPOComboBox xpoComboBoxPriceType = new XPOComboBox(Entity.Session, typeof(fin_configurationpricetype), (Entity as pos_configurationplace).PriceType, "Designation", null);
                BOWidgetBox boxPriceType = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_placetable_PriceType"), xpoComboBoxPriceType);
                vboxTab1.PackStart(boxPriceType, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPriceType, Entity, "PriceType", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //MovementType
                XPOComboBox xpoComboBoxMovementType = new XPOComboBox(Entity.Session, typeof(pos_configurationplacemovementtype), (Entity as pos_configurationplace).MovementType, "Designation", null);
                BOWidgetBox boxMovementType = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_placetable_MovementType"), xpoComboBoxMovementType);
                vboxTab1.PackStart(boxMovementType, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxMovementType, Entity, "MovementType", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //IN009269 Criar campo inserir imagem para locais
                //ButtonImage
                FileChooserButton fileChooserButtonImage = new FileChooserButton(string.Empty, FileChooserAction.Open) { HeightRequest = 23 };
                Image fileChooserImagePreviewButtonImage = new Image() { WidthRequest = _sizefileChooserPreview.Width, HeightRequest = _sizefileChooserPreview.Height };
                Frame fileChooserFrameImagePreviewButtonImage = new Frame();
                fileChooserFrameImagePreviewButtonImage.ShadowType = ShadowType.None;
                fileChooserFrameImagePreviewButtonImage.Add(fileChooserImagePreviewButtonImage);
                fileChooserButtonImage.SetFilename(((pos_configurationplace)Entity).ButtonImage);
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
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = LogicPOSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
