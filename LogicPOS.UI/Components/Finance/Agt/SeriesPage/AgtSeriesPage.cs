using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Finance.Agt.RequestSeriesModal;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtSeriesPage : Page<OnlineSeriesInfo>
    {
        public AgtSeriesPage(Window parent) : base(parent)
        {
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
            Navigator.SearchBox.BtnFilter.Visible = false;
            Navigator.SearchBox.BtnMore.Visible = false;
            
        }
        protected override IRequest<ErrorOr<IEnumerable<OnlineSeriesInfo>>> GetAllQuery => new ListOnlineSeriesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            if(SelectedEntity != null && mode != EntityEditionModalMode.Insert)
            {
                SeriesInfoModal.Show(SelectedEntity, this.SourceWindow);
                return 0;
            }

            var modal = new RequestSeriesModal();
            var response = modal.Run();
            modal.Destroy();
            return response;
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

                var entity = model.GetValue(iterator, 0) as OnlineSeriesInfo;

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
