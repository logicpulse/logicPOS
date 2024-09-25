using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocumentModal;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogicPOS.UI.Components.Modals
{
    public class CreateDocumentModal : Modal
    {
        #region Buttons
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
         
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnPreview { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png");
     
        string BtnPreviewIcon => PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png";
        string BtnClearCustomerIcon => PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";
        #endregion

        private ModalTabsNavigator Navigator { get; set; } 

        public CreateDocumentModal(Window parent) : base(parent: parent,
                                                         title: GeneralUtils.GetResourceByName("window_title_dialog_new_finance_document"),
                                                         size: new System.Drawing.Size(790, 546),
                                                         icon: PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png")
        {

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnClearCustomer,  (ResponseType)12),
                new ActionAreaButton(BtnPreview, (ResponseType)11),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };

            return actionAreaButtons;
        }

        protected override Widget CreateBody()
        {
            Navigator = new ModalTabsNavigator(new CreateDocumentDocumentTab(this),
                                                new CreateDocumentCustomerTab(this),
                                                new CreateDocumentArticlesTab(this),
                                                new CreateDocumentShipToTab(this),
                                                new CreateDocumentShipFromTab(this));

            VBox boxContent = new VBox();
            boxContent.PackStart(Navigator, true, true, 0);
            return boxContent;
        }
    }
}
