using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class UsersMenu : Menu<User>
    {
        private Size ButtonSize { get; } = new Size(120, 102);
        private string ButtonName => "buttonUserId";

        public UsersMenu(uint rows,
                         uint columns,
                         CustomButton btnPrevious,
                         CustomButton btnNext,
                         Window sourceWindow) : base(rows,
                                                     columns,
                                                     btnPrevious: btnPrevious,
                                                     btnNext: btnNext,
                                                     sourceWindow: sourceWindow)
        {
            SelectFirstOnReload = true;
            Refresh();
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            var users = UsersService.GetAllUsers();

            if (users != null)
            {
                Entities.AddRange(users);
            }
        }

        public static IconButton CreatePreviousButton()
        {
            var buttonSize = AppSettings.Instance.SizeStartupWindowObjectsTablePadUserButton;

            IconButton button = new IconButton(
                   new ButtonSettings
                   {
                       Name = "TablePadUserButtonPrev",
                       Icon = @"Assets\Images\Buttons\Pos\button_family_scroll_up.png",
                       IconSize = new Size(buttonSize.Width - 2, buttonSize.Height - 2),
                       ButtonSize = buttonSize
                   });

            button.Relief = ReliefStyle.None;
            button.BorderWidth = 0;
            button.CanFocus = false;

            return button;
        }

        protected override CustomButton CreateButtonForEntity(User entity)
        {
            return MenuButton<User>.CreateButton(ButtonName, entity.Name, AppSettings.Paths.Images + @"Icons\Users\icon_user_default.png", ButtonSize);
        }
    }
}
