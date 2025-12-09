using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PreferenceParametersPage : Page<PreferenceParameter>
    {
        public static readonly Dictionary<string, string> CompanyParametersOptions = new Dictionary<string, string> { { "parameters", "company" } };
        public static readonly Dictionary<string, string> SystemParametersOptions = new Dictionary<string, string> { { "parameters", "system" } };

        public PreferenceParametersPage(Gtk.Window parent, Dictionary<string, string> options) : base(parent, options)
        {
            CanDeleteEntity = false;
            Navigator.BtnInsert.Sensitive = false;
            Navigator.Update();
            DisableFilterButton();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateValueColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
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

                var designation = parameter?.ResourceStringValue;
                if (designation != null && designation.ToLower().Contains(search))
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
        protected override void LoadEntities()
        {
            var getParametersResult = _mediator.Send(new GetAllPreferenceParametersQuery()).Result;

            if (getParametersResult.IsError)
            {
                HandleErrorResult(getParametersResult);
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

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PreferenceParameterModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_EDIT");
        }

        #region Singleton
        private static PreferenceParametersPage _companyParametersInstance;
        private static PreferenceParametersPage _systemParametersInstance;

        public static PreferenceParametersPage CompanyPageInstance
        {
            get
            {
                if (_companyParametersInstance == null)
                {
                    _companyParametersInstance = new PreferenceParametersPage(BackOfficeWindow.Instance,
                                                                              CompanyParametersOptions);
                }

                return _companyParametersInstance;
            }
        }

        public static PreferenceParametersPage SystemPageInstance
        {
            get
            {
                if (_systemParametersInstance == null)
                {
                    _systemParametersInstance = new PreferenceParametersPage(BackOfficeWindow.Instance,
                                                                             SystemParametersOptions);
                }

                return _systemParametersInstance;
            }
        }
        #endregion

    }
}
