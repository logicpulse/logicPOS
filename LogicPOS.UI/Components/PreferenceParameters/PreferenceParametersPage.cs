using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public class PreferenceParametersPage : Page<PreferenceParameter>
    {
        public PreferenceParametersPage(Window parent, Dictionary<string,string> options) : base(parent,options)
        {
            CanDeleteEntity = false;
            Navigator.ButtonInsert.Sensitive = false;
            Navigator.Update();
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateValueColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
        }

        private TreeViewColumn CreateValueColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var parameter = (PreferenceParameter)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = parameter.Value;
            }

            var title = GeneralUtils.GetResourceByName("global_value");
            var valueColumn = Columns.CreateColumn(title, 1, RenderValue);

            return valueColumn;
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var parameter = (PreferenceParameter)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = parameter.ResourceStringValue;
            }

            var title = GeneralUtils.GetResourceByName("global_designation");

            var column = Columns.CreateColumn(title,
                                              0,
                                              RenderDesignation);
            column.MinWidth = 250;
            column.MaxWidth = 800;

            return column;
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                search = search.Trim();
                var parameter = (PreferenceParameter)model.GetValue(iterator, 0);

                var designation = parameter.ResourceStringValue;
                if (designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddDesignationSorting();
            AddValueSorting();
            AddUpdatedAtSorting(2);
        }

        private void AddValueSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, a, b) =>
            {
                var parameterA = (PreferenceParameter)model.GetValue(a, 0);
                var parameterB = (PreferenceParameter)model.GetValue(b, 0);

                if ((parameterA == null || parameterB == null) || (parameterA.Value == null || parameterB.Value == null))
                {
                    return 0;
                }

                return parameterA.Value.CompareTo(parameterB.Value);
            });
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, a, b) =>
            {
                var parameterA = (PreferenceParameter)model.GetValue(a, 0);
                var paramterB = (PreferenceParameter)model.GetValue(b, 0);

                if (parameterA == null || paramterB == null)
                {
                    return 0;
                }

                return parameterA.ResourceStringValue.CompareTo(paramterB.ResourceStringValue);
            });
        }

        protected override void LoadEntities()
        {
            var getParametersResult = _mediator.Send(new GetAllPreferenceParametersQuery()).Result;

            if (getParametersResult.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _entities.Clear();


            List<PreferenceParameter> parameters;

            if (Options["parameters"] == "company")
            {
                parameters = getParametersResult.Value.Where(p => p.FormType == 1).ToList();
            }
            else
            {
                parameters = getParametersResult.Value.Where(p => p.FormType == 2).ToList();
            }

            _entities.AddRange(parameters);
            _entities.ForEach(p => p.ResourceStringValue = GeneralUtils.GetResourceByName(p.ResourceString));
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new PreferenceParameterModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }
    }
}
