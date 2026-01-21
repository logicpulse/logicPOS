using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CreateDocumentModal
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            if (SystemInformationService.SystemInformation.IsAngola)
            {
                actionAreaButtons.Add(new ActionAreaButton(BtnAgtNifInfo, ResponseType.None));
            }

            actionAreaButtons.Add(new ActionAreaButton(BtnClear, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnPreview, ResponseType.None));
            actionAreaButtons.Add(new ActionAreaButton(BtnOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnCancel, ResponseType.Cancel));

            return actionAreaButtons;
        }

        protected override Widget CreateBody()
        {
            InitializeNavigator();
            VBox box = new VBox();
            box.PackStart(Navigator, true, true, 0);
            return box;
        }

        protected override Widget CreateLeftContent()
        {
            var hbox = new HBox(true, 0);
            CheckIsDraft.Child.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.FontEntryBoxLabel));
            hbox.PackStart(CheckIsDraft, false, false, 0);
            return hbox;
        }
    }
}
