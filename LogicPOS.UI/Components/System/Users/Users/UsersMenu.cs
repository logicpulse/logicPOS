using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.GetAllUsers;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class UsersMenu : Menu<User>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public UsersMenu(uint rows,
                         uint columns,
                         CustomButton btnPrevious,
                         CustomButton btnNext,
                         Window sourceWindow) : base(rows,
                                                     columns,
                                                     buttonSize: new Size(120, 102),
                                                     buttonName: "buttonUserId",
                                                     btnPrevious: btnPrevious,
                                                     btnNext: btnNext,
                                                     sourceWindow: sourceWindow)
        {

        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            var getUsersResult = _mediator.Send(new GetAllUsersQuery()).Result;

            if (getUsersResult.IsError == false)
            {
                Entities.AddRange(getUsersResult.Value);
            }
        }

        protected override string GetButtonLabel(User user)
        {
            return user.Name;
        }

        protected override string GetButtonImage(User user)
        {
            return PathsSettings.ImagesFolderLocation + @"Icons\Users\icon_user_default.png";
        }

        public static IconButton CreatePreviousButton()
        {
            var buttonSize = AppSettings.Instance.sizeStartupWindowObjectsTablePadUserButton;

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

        protected override IEnumerable<User> FilterEntities(IEnumerable<User> entities)
        {
            return entities;
        }
    }
}
