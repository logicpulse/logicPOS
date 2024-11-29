using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.MeasurementUnits.DeleteMeasurementUnit;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.MeasurementUnits.GetAllMeasurementUnits;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class MeasurementUnitsPage : Page<MeasurementUnit>
    {
        public MeasurementUnitsPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<MeasurementUnit>>> GetAllQuery => new GetAllMeasurementUnitsQuery();
       
        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new MeasurementUnitModal(mode, SelectedEntity as MeasurementUnit);
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
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var measurementUnit = (MeasurementUnit)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = measurementUnit.Acronym.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftMeasurementUnit = (MeasurementUnit)model.GetValue(left, 0);
                var rightMeasurementUnit = (MeasurementUnit)model.GetValue(right, 0);

                if (leftMeasurementUnit == null || rightMeasurementUnit == null)
                {
                    return 0;
                }

                return leftMeasurementUnit.Acronym.CompareTo(rightMeasurementUnit.Acronym);
            });
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteMeasurementUnitCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static MeasurementUnitsPage _instance;
        public static MeasurementUnitsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MeasurementUnitsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }

}
