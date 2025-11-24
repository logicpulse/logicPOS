using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.ListSeries;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Modals;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtSeriesPage : Page<AgtSeriesInfo>
    {
        public AgtSeriesPage(Window parent) : base(parent)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
        }

        protected override IRequest<ErrorOr<IEnumerable<AgtSeriesInfo>>> GetAllQuery => new ListAgtSeriesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            if(SelectedEntity != null)
            {
                SeriesInfoModal.Show(SelectedEntity, this.SourceWindow);
            }

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

                var entity = model.GetValue(iterator, 0) as AgtSeriesInfo;

                return entity != null && entity.Code.ToLower().Contains(search);
            };
        }


        #region Singleton
        private static AgtSeriesPage _instance;
        public static AgtSeriesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AgtSeriesPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
