using System;
using Gtk;
using System.Drawing;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosChangeUserDialog : PosBaseDialog
    {
        //Settings
        //Sizes
        private Size _sizePosSmallButtonScroller = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizePosSmallButtonScroller"]);
        private Size _sizePosUserButton = logicpos.Utils.StringToSize(GeneralSettings.Settings["sizePosUserButton"]);
        private Size _sizeIconScrollLeftRight = new Size(62, 31);
        //Files
        private readonly string _fileScrollLeftImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_left.png";
        private readonly string _fileScrollRightImage = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_subfamily_article_scroll_right.png";

        //Private Gui Members
        private readonly Fixed _fixedContent;
        private TablePad _tablePadUsers;

        //TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;

        public sys_userdetail UserDetail { get; set; }

        public PosChangeUserDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_change_user");
            Size windowSize = new Size(559, 562);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_users.png";

            //Init Content
            _fixedContent = new Fixed();

            InitTablePadUsers();

            //ActionArea Buttons
            //_buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok) { Sensitive = false };
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                //actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitTablePadUsers()
        {
            //Colors
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(LogicPOS.Settings.GeneralSettings.Settings["colorPosButtonArticleBackground"]);

            //Scrollers
            TouchButtonIcon buttonPosScrollersPlacePrev = new TouchButtonIcon("buttonPosScrollersTablePrev", Color.White, _fileScrollLeftImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
            TouchButtonIcon buttonPosScrollersPlaceNext = new TouchButtonIcon("buttonPosScrollersTableNext", Color.White, _fileScrollRightImage, _sizeIconScrollLeftRight, _sizePosSmallButtonScroller.Width, _sizePosSmallButtonScroller.Height);
            buttonPosScrollersPlacePrev.Relief = ReliefStyle.None;
            buttonPosScrollersPlaceNext.Relief = ReliefStyle.None;
            buttonPosScrollersPlacePrev.BorderWidth = 0;
            buttonPosScrollersPlaceNext.BorderWidth = 0;
            buttonPosScrollersPlacePrev.CanFocus = false;
            buttonPosScrollersPlaceNext.CanFocus = false;
            HBox hboxPlaceScrollers = new HBox(true, 0);
            hboxPlaceScrollers.PackStart(buttonPosScrollersPlacePrev);
            hboxPlaceScrollers.PackStart(buttonPosScrollersPlaceNext);

            //TablePad Places
            string sqlUsers = @"SELECT Oid as id, Name as name, NULL as label, NULL as image FROM sys_userdetail WHERE (Disabled IS NULL or Disabled  <> 1)";
            _tablePadUsers = new TablePadUser(
                sqlUsers, 
                "ORDER BY Ord", 
                "", 
                XPOSettings.LoggedUser.Oid, 
                true,
                5, 
                4, 
                "buttonUserId", 
                Color.Transparent, 
                _sizePosUserButton.Width, 
                _sizePosUserButton.Height, 
                buttonPosScrollersPlacePrev, 
                buttonPosScrollersPlaceNext
            );
            //Click Event
            _tablePadUsers.Clicked += _tablePadUsers_Clicked;
            //Pack It
            _fixedContent.Put(_tablePadUsers, 0, 0);
            _fixedContent.Put(hboxPlaceScrollers, 0, 411);
        }

        private void _tablePadUsers_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadUsers.SelectedButtonOid = button.CurrentButtonOid;
            //To be Used in Dialog Result
            UserDetail = XPOUtility.GetEntityById<sys_userdetail>(button.CurrentButtonOid);

            if (UserDetail.PasswordReset)
            {
                //_logger.Debug(string.Format("Name: [{0}], PasswordReset: [{1}]", _selectedUserDetail.Name, _selectedUserDetail.PasswordReset));
                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"),
                    string.Format(GeneralUtils.GetResourceByName("dialog_message_user_request_change_password"), UserDetail.Name, XPOSettings.DefaultValueUserDetailAccessPin)
                );
            }

            //Send Response to Replace the Old Ok Button
            Respond(ResponseType.Ok);
        }
    }
}
