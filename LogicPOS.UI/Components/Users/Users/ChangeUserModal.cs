using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class ChangeUserModal : BaseDialog
    {
        private Size _sizePosSmallButtonScroller = AppSettings.Instance.sizePosSmallButtonScroller;
        private Size _sizePosUserButton = AppSettings.Instance.sizePosUserButton;
        private Size _sizeIconScrollLeftRight = new Size(62, 31);
        private readonly string _fileScrollLeftImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png";
        private readonly string _fileScrollRightImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png";
        private readonly Fixed _fixedContent;
        private UsersMenu UsersMenu { get; set; }
        private readonly IconButtonWithText _buttonCancel;
        public UserDetail User { get; set; }

        public ChangeUserModal(Window parentWindow,
                                   DialogFlags pDialogFlags)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_change_user");
            Size windowSize = new Size(559, 562);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_users.png";

            _fixedContent = new Fixed();

            InitUsersMenu();

            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            this.Initialize(this,
                            pDialogFlags,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            _fixedContent,
                            actionAreaButtons);
        }

        private void InitUsersMenu()
        {
            IconButton btnPrevious = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonPosScrollersTablePrev",
                    BackgroundColor = Color.White,
                    Icon = _fileScrollLeftImage,
                    IconSize = _sizeIconScrollLeftRight,
                    ButtonSize = _sizePosSmallButtonScroller
                });

            IconButton btnNext = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonPosScrollersTableNext",
                    BackgroundColor = Color.White,
                    Icon = _fileScrollRightImage,
                    IconSize = _sizeIconScrollLeftRight,
                    ButtonSize = _sizePosSmallButtonScroller
                });

            btnPrevious.Relief = ReliefStyle.None;
            btnNext.Relief = ReliefStyle.None;
            btnPrevious.BorderWidth = 0;
            btnNext.BorderWidth = 0;
            btnPrevious.CanFocus = false;
            btnNext.CanFocus = false;
            HBox hboxPlaceScrollers = new HBox(true, 0);
            hboxPlaceScrollers.PackStart(btnPrevious);
            hboxPlaceScrollers.PackStart(btnNext);

            UsersMenu = new UsersMenu(
                this,
                btnPrevious,
                btnNext,
                5,
                4
            );

            UsersMenu.OnUserSelected += OnUserSelectd;
            _fixedContent.Put(UsersMenu, 0, 0);
            _fixedContent.Put(hboxPlaceScrollers, 0, 411);
        }

        private void OnUserSelectd(UserDetail user)
        {
            User = user;

            if (User.PasswordReset)
            {
                logicpos.Utils.ShowMessageTouch(this,
                                                DialogFlags.Modal,
                                                MessageType.Info,
                                                ButtonsType.Ok,
                                                GeneralUtils.GetResourceByName("global_information"),
                                                string.Format(GeneralUtils.GetResourceByName("dialog_message_user_request_change_password"), User.Name, XPOSettings.DefaultValueUserDetailAccessPin));
            }

            Respond(ResponseType.Ok);
        }
    }
}
