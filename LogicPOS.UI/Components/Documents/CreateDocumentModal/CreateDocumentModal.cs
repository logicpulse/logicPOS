using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.IO;

namespace LogicPOS.UI.Components.Modals
{
    public class CreateDocumentModal : Dialog
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClearCustomer { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonClearCustomer_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("global_button_label_payment_dialog_clear_client"),
         
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png");
        private IconButtonWithText BtnPreview { get; set; } = ActionAreaButton.FactoryGetDialogButtonType("touchButtonPreview_DialogActionArea",
                                                                                                          GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_preview"),
                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png");
        
        public EntitySelectionModalWindowSettings WindowSettings { get; set; } = new EntitySelectionModalWindowSettings();
        string BtnPreviewIcon => PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_preview.png";
        string BtnClearCustomerIcon => PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";

        public CreateDocumentModal(Window parent, DialogFlags flags) : base("",parent, flags)
        {
            WindowSettings.Size = new System.Drawing.Size(790, 546);
            WindowSettings.Source = parent;
            WindowSettings.Icon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png";

            InitializeWindow();

            Design();

            ShowAll();

            ActionArea.Visible = false;
        }


        public void Design()
        {
            DesignTitle();
            DesignMinimizeButton();
            DesignCloseButton();

            VBox vboxWindow = new VBox(false, 0) { BorderWidth = WindowSettings.BorderWidth };
            vboxWindow.PackStart(CreateTitleEventBox(), false, false, 0);
            vboxWindow.PackStart(CreateContentEventBox(), true, true, 0);
            vboxWindow.PackStart(CreateActionAreaBox(), false, false, 0);

            EventBox eventboxWindowBorderInner = new EventBox() { BorderWidth = 0 };
            eventboxWindowBorderInner.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackground.ToGdkColor());
            eventboxWindowBorderInner.Add(vboxWindow);

            EventBox eventboxWindowBorderOuter = new EventBox();
            eventboxWindowBorderOuter.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackgroundBorder.ToGdkColor());
            eventboxWindowBorderOuter.Add(eventboxWindowBorderInner);

            VBox.PackStart(eventboxWindowBorderOuter, true, true, 0);
        }

        private void InitializeWindow()
        {
            WindowSettings.Title = new Label(GeneralUtils.GetResourceByName("window_title_dialog_new_finance_document"));

            Modal = false;
            Decorated = false;
            Resizable = false;
            Fixed fixedContent = new Fixed();

            WindowSettings.Content = fixedContent;
            WindowSettings.RightButtons = CreateActionAreaButtons();
            WindowPosition = WindowPosition.Center;
            SetSizeRequest(WindowSettings.Size.Width, WindowSettings.Size.Height);
            DefaultResponse = ResponseType.Cancel;
            ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackgroundBorder.ToGdkColor());
            TransientFor = WindowSettings.Source;
        }

        public ActionAreaButtons CreateActionAreaButtons()
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

        private Gtk.Image CreateMinimizeIcon()
        {
            Gdk.Pixbuf icon = new Gdk.Pixbuf(EntitySelectionModalIcons.MinimizeWindowIcon);
            Gtk.Image image = new Gtk.Image(icon);
            return image;
        }

        private Gtk.Image CreateCloseIcon()
        {
            Gdk.Pixbuf icon = new Gdk.Pixbuf(EntitySelectionModalIcons.CloseWindowIcon);
            Gtk.Image image = new Gtk.Image(icon);
            return image;
        }

        private void DesignMinimizeButton()
        {
            var icon = CreateMinimizeIcon();
            WindowSettings.MinimizeWindow = new EventBox();
            WindowSettings.MinimizeWindow.WidthRequest = icon.Pixbuf.Width;
            WindowSettings.MinimizeWindow.Add(icon);
            WindowSettings.MinimizeWindow.VisibleWindow = false;

            WindowSettings.MinimizeWindow.ButtonReleaseEvent += delegate
            {
                Respond(ResponseType.None);
            };
        }

        private void DesignCloseButton()
        {
            var icon = CreateCloseIcon();
            WindowSettings.CloseWindow = new EventBox();
            WindowSettings.CloseWindow.WidthRequest = icon.Pixbuf.Width;
            WindowSettings.CloseWindow.Add(icon);
            WindowSettings.CloseWindow.VisibleWindow = false;

            WindowSettings.CloseWindow.ButtonPressEvent += (o, args) =>
            {
                if (this is PosKeyboardDialog)
                {
                    if (WindowSettings.UseMask) WindowSettings.Mask.Hide();
                    Respond(ResponseType.Close);
                }
                else
                {
                    Destroy();
                }
            };
        }

        private HBox CreateTitleBar()
        {
            HBox box = new HBox(false, 0);
            box.PackStart(CreateWindowIcon(), false, false, 2);
            box.PackStart(WindowSettings.Title, true, true, 2);

            if (WindowSettings.ShowCloseButton)
            {
                box.PackStart(WindowSettings.CloseWindow, false, false, 2);
            }

            return box;
        }

        private void DesignTitle()
        {
            Pango.FontDescription fontDescription = new Pango.FontDescription();
            fontDescription.Weight = Pango.Weight.Bold;
            fontDescription.Size = 18;

            WindowSettings.Title.SetAlignment(0, 0.5F);
            WindowSettings.Title.ModifyFg(StateType.Normal, System.Drawing.Color.White.ToGdkColor());
            WindowSettings.Title.ModifyFont(fontDescription);
        }

        private EventBox CreateTitleEventBox()
        {
            EventBox title = new EventBox();
            title.ModifyBg(StateType.Normal, EntitySelectionModalColors.TitleBackground.ToGdkColor());
            title.HeightRequest = 40;
            title.Add(CreateTitleBar());

            title.ButtonPressEvent += WindowStartDrag;
            title.ButtonReleaseEvent += WindowEndDrag;
            title.MotionNotifyEvent += WindowMotionDrag;

            return title;
        }

        private Gtk.Image CreateWindowIcon()
        {
            Gdk.Pixbuf pixbufWindowIcon = new Gdk.Pixbuf(EntitySelectionModalIcons.WindowIcon);

            if (WindowSettings.Icon != string.Empty && File.Exists(WindowSettings.Icon))
            {
                pixbufWindowIcon = new Gdk.Pixbuf(WindowSettings.Icon);
            }

            Gtk.Image imageWindowsIcon = new Gtk.Image(pixbufWindowIcon);

            return imageWindowsIcon;
        }

        private EventBox CreateContentEventBox()
        {
            EventBox box = new EventBox();
            box.ModifyBg(StateType.Normal, EntitySelectionModalColors.WindowBackground.ToGdkColor());
            box.Add(WindowSettings.Content);

            return box;
        }

        private HBox CreateActionAreaBox()
        {
            HBox box = new HBox(false, 0);

            if (WindowSettings.LeftContent != null)
            {
                box.PackStart(WindowSettings.LeftContent, false, false, 0);
            }

            box.PackStart(new HBox(), true, true, 0);
            box.PackStart(CreateRightButtonsBox(), false, false, 0);

            return box;
        }

        private HBox CreateRightButtonsBox()
        {
            HBox box = new HBox() { HeightRequest = 60 };

            if (WindowSettings.RightButtons != null && WindowSettings.RightButtons.Count > 0)
            {
                box = GetActionAreaButtonsBox(WindowSettings.RightButtons);
            };

            return box;
        }

        private HBox GetActionAreaButtonsBox(ActionAreaButtons buttons)
        {
            int position;
            HBox box = new HBox(false, 5) { Direction = TextDirection.Rtl };
            for (int i = 0; i < buttons.Count; i++)
            {
                position = buttons.Count - 1 - i;
                buttons[position].Clicked += Button_Clicked;
                box.PackStart(buttons[position].Button, false, false, 0);

                if (WindowSettings.ConfirmButton == null)
                {
                    switch (buttons[position].Response)
                    {
                        case ResponseType.Accept:
                        case ResponseType.Apply:
                        case ResponseType.Ok:
                        case ResponseType.Yes:
                        case ResponseType.Close:

                            WindowSettings.ConfirmButton = buttons[position].Button;
                            break;
                    }
                }
            }
            return box;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ActionAreaButton button = (ActionAreaButton)sender;
            Respond(button.Response);
        }

        protected void WindowStartDrag(object o, ButtonPressEventArgs args)
        {
            int windowX, windowY, mouseX, mouseY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            GetPosition(out windowX, out windowY);
            WindowSettings.DragOffsetX = mouseX - windowX;
            WindowSettings.DragOffsetY = mouseY - windowY;
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Cross);
        }

        protected void WindowEndDrag(object o, ButtonReleaseEventArgs args)
        {
            GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
        }

        protected virtual void WindowMotionDrag(object o, MotionNotifyEventArgs args)
        {
            int mouseX, mouseY, windowX, windowY, moveX, moveY, currentX, currentY;
            Gdk.Display.Default.GetPointer(out mouseX, out mouseY);
            Display.GetPointer(out windowX, out windowY);
            moveX = windowX - WindowSettings.DragOffsetX;
            moveY = windowY - WindowSettings.DragOffsetY;
            Move(moveX, moveY);
            GetPosition(out currentX, out currentY);
        }

        public void HideCloseButton() => WindowSettings.ShowCloseButton = false;

        private void ApplyMask(Window parent)
        {
            //Window Mask Background Hack
            WindowSettings.Mask = new Window("");
            WindowSettings.Mask.TransientFor = parent;
            WindowSettings.Mask.SetSizeRequest(10, 10);
            WindowSettings.Mask.Move(-100, -100);
            WindowSettings.Mask.ModifyBg(StateType.Normal, System.Drawing.Color.Black.ToGdkColor());

            //Prevent click outside Dialog
            WindowSettings.Mask.Opacity = 0.35F;//0.55F | 0.75F
            WindowSettings.Mask.CanFocus = false;
            WindowSettings.Mask.AcceptFocus = false;
            WindowSettings.Mask.Sensitive = false;
            WindowSettings.Mask.Fullscreen();
            WindowSettings.Mask.Show();

            TransientFor = WindowSettings.Mask;

            Destroyed += delegate { WindowSettings.Mask.Destroy(); };
        }
    }
}
