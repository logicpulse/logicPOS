using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Modals;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtDocumentsPage : Page<OnlineDocument>
    {
        public AgtDocumentsPage(Window parent) : base(parent)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
        }

        protected override IRequest<ErrorOr<IEnumerable<OnlineDocument>>> GetAllQuery =>
            new ListOnlineDocumentsQuery(DateTime.Today.AddDays(-90), DateTime.Today);

        public override int RunModal(EntityEditionModalMode mode)
        {
            if (SelectedEntity == null)
            {
                return 0;
            }

            AgtOnlineDocumentInfoModal.Show(SelectedEntity.Number, SourceWindow);

            return 0;
        }

        public override void UpdateButtonPrevileges() { }

        protected override DeleteCommand GetDeleteCommand() => null;

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as OnlineDocument;

                return entity != null && (
                    (!string.IsNullOrEmpty(entity.Number) && entity.Number.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(entity.Type) && entity.Type.ToLower().Contains(search))
                );
            };
        }

        #region Singleton
        private static AgtDocumentsPage _instance;
        public static AgtDocumentsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AgtDocumentsPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
