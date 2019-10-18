using System;
using Gtk;
using System.Drawing;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosChangeUserDialog : PosBaseDialog
    {
        //Settings
        //Sizes
        private Size _sizePosSmallButtonScroller = Utils.StringToSize(GlobalFramework.Settings["sizePosSmallButtonScroller"]);
        private Size _sizePosUserButton = Utils.StringToSize(GlobalFramework.Settings["sizePosUserButton"]);
        private Size _sizeIconScrollLeftRight = new Size(62, 31);
        //Files
        private String _fileScrollLeftImage = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_subfamily_article_scroll_left.png");
        private String _fileScrollRightImage = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Buttons\Pos\button_subfamily_article_scroll_right.png");
        //Private Gui Members
        Fixed _fixedContent;
        TablePad _tablePadUsers;
        //TouchButtonIconWithText _buttonOk;
        TouchButtonIconWithText _buttonCancel;
        //Public Properties
        private sys_userdetail _selectedUserDetail;
        public sys_userdetail UserDetail
        {
            get { return _selectedUserDetail; }
            set { _selectedUserDetail = value; }
        }

        public PosChangeUserDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_change_user");
            Size windowSize = new Size(559, 562);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_users.png");

            //Init Content
            _fixedContent = new Fixed();

            InitTablePadUsers();

            //ActionArea Buttons
            //_buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok) { Sensitive = false };
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitTablePadUsers()
        {
            //Colors
            //Color colorPosButtonArticleBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorPosButtonArticleBackground"]);

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
            String sqlUsers = @"SELECT Oid as id, Name as name, NULL as label, NULL as image FROM sys_userdetail WHERE (Disabled IS NULL or Disabled  <> 1)";
            _tablePadUsers = new TablePadUser(
                sqlUsers, 
                "ORDER BY Ord", 
                "", 
                GlobalFramework.LoggedUser.Oid, 
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

        void _tablePadUsers_Clicked(object sender, EventArgs e)
        {
            TouchButtonBase button = (TouchButtonBase)sender;

            //Assign CurrentId to TablePad.CurrentId, to Know last Clicked Button Id
            _tablePadUsers.SelectedButtonOid = button.CurrentButtonOid;
            //To be Used in Dialog Result
            _selectedUserDetail = (sys_userdetail)FrameworkUtils.GetXPGuidObject(typeof(sys_userdetail), button.CurrentButtonOid);

            if (_selectedUserDetail.PasswordReset)
            {
                //_log.Debug(string.Format("Name: [{0}], PasswordReset: [{1}]", _selectedUserDetail.Name, _selectedUserDetail.PasswordReset));
                Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"),
                    string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_user_request_change_password"), _selectedUserDetail.Name, SettingsApp.DefaultValueUserDetailAccessPin)
                );
            }

            //Send Response to Replace the Old Ok Button
            Respond(ResponseType.Ok);
        }
    }
}
