using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.VatExcemptionReasons.DeleteVatExcemptionReason;
using LogicPOS.Api.Features.VatExemptionReasons.GetAllVatExemptionReasons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class VatExemptionReasonsPage : Page<VatExemptionReason>
    {
        protected override IRequest<ErrorOr<IEnumerable<VatExemptionReason>>> GetAllQuery => new GetAllVatExemptionReasonsQuery();
        public VatExemptionReasonsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new VatExemptionReasonModal(mode, SelectedEntity as VatExemptionReason);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderAcronym(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var vatRate = (VatExemptionReason)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = vatRate.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderAcronym);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteVatExemptionReasonCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static VatExemptionReasonsPage _instance;
        public static VatExemptionReasonsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VatExemptionReasonsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
