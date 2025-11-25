using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.ListInvoices;
using LogicPOS.UI.Components.Modals;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtDocumentsPage : Page<AgtInvoice>
    {
        public AgtDocumentsPage(Window parent) : base(parent)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
        }

        protected override IRequest<ErrorOr<IEnumerable<AgtInvoice>>> GetAllQuery =>
            new ListInvoicesQuery(DateTime.Today.AddDays(-90), DateTime.Today);

        public override int RunModal(EntityEditionModalMode mode)
        {
            // No modal implemented for AGT invoices yet
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

                var entity = model.GetValue(iterator, 0) as AgtInvoice;

                return entity != null && (
                    (!string.IsNullOrEmpty(entity.DocumentNumber) && entity.DocumentNumber.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(entity.DocumentType) && entity.DocumentType.ToLower().Contains(search))
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
