using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public class EntitySelectionModal<TEntity> : Modal where TEntity : ApiEntity
    {
        public Size SelectionPageSize => new Size(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
        public Page<TEntity> Page { get; private set; }

        public EntitySelectionModal(
            Page<TEntity> page,
            string title) : base(page.SourceWindow,
                                 title,
                                 LogicPOSAppContext.MaxWindowSize,
                                 $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}",
                                 render: false)
        {
            Page = page;
            Page.SetSizeRequest(SelectionPageSize.Width, SelectionPageSize.Height);
            Render();
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            var btnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            var btnCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(btnOk, ResponseType.Ok),
                new ActionAreaButton(btnCancel, ResponseType.Cancel)
            };
            return actionAreaButtons;
        }

        protected override Widget CreateBody()
        {
            Fixed fixedContent = new Fixed();
            fixedContent.Put(Page, 0, 0);
            return fixedContent;
        }

        protected override Widget CreateLeftContent()
        {
            return CreateSearchBox();
        }

        private Widget CreateSearchBox()
        {
            var searchBox = new PageSearchBox(Page.SourceWindow, true);
            searchBox.TxtSearch.EntryValidation.Changed += delegate
            {
                Page.Navigator.SearchBox.TxtSearch.EntryValidation.Text = searchBox.TxtSearch.EntryValidation.Text;
            };

            return searchBox;
        }

    }
}
