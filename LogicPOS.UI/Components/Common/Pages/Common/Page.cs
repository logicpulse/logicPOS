using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public abstract class Page<TEntity> : Box where TEntity : ApiEntity
    {
        protected readonly ISender _mediator = DependencyInjection.Mediator;
        protected readonly List<TEntity> _entities = new List<TEntity>();
        protected virtual IRequest<ErrorOr<IEnumerable<TEntity>>> GetAllQuery { get; set; }

        public Window SourceWindow { get; }
        public TreeView GridView { get; set; }
        public TEntity SelectedEntity { get; set; }
        public event Action<TEntity> EntitySelected;
        public event Action<TEntity> SelectedEntityConfirmed;

        public bool CanViewEntity { get; set; } = true;
        public bool CanUpdateEntity { get; set; } = true;
        public bool CanDeleteEntity { get; set; } = true;

        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();
        internal PageNavigator<TEntity> Navigator { get; }
        public Dictionary<string, string> Options { get; private set; }

        public Page(Window parent, Dictionary<string, string> options = null)
        {
            SourceWindow = parent;
            Navigator = new PageNavigator<TEntity>(this);
            Options = options;

            LoadEntities();

            InitializeGridView();

            Design();

            AddEntitiesToModel(_entities);

            ShowAll();

            Navigator.Update();
        }

        protected void HandleErrorResult<TResult>(ErrorOr<TResult> result) => ErrorHandlingService.HandleApiError(result, source: SourceWindow);

        public virtual void Refresh()
        {
            LoadEntities();
            ClearGridViewModel();
            AddEntitiesToModel(_entities);
        }

        protected void ClearGridViewModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
        }

        protected abstract DeleteCommand GetDeleteCommand();

        public virtual bool DeleteEntity()
        {
            var command = GetDeleteCommand();

            if (command == null)
            {
                return false;
            }

            var result = _mediator.Send(GetDeleteCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                return false;
            }

            if (result.Value == false)
            {
                CustomAlerts.ShowCannotDeleteEntityErrorAlert(SourceWindow);
            }

            return result.Value;
        }

        protected virtual void LoadEntities()
        {
            var getEntitiesResult = _mediator.Send(GetAllQuery).Result;

            if (getEntitiesResult.IsError)
            {
                HandleErrorResult(getEntitiesResult);
                return;
            }
            
            _entities.Clear();
            _entities.AddRange(getEntitiesResult.Value);
        }

        protected virtual void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(TEntity));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }

        protected void InitializeGridViewModel()
        {
            InitializeFilter();
            InitializeSort();
        }

        protected virtual void AddEntitiesToModel(IEnumerable<TEntity> entities)
        {
            var model = (ListStore)GridViewSettings.Model;

            foreach (var entity in entities)
            {
                model.AppendValues(entity);
            }
        }

        protected virtual void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as TEntity;

                if (entity != null && ((entity as IWithDesignation) != null))
                {
                    if (entity != null && (entity as IWithDesignation).Designation.ToLower().Contains(search))
                    {
                        return true;
                    }
                }

                if (entity != null && ((entity as IWithName) != null))
                {
                    if (entity != null && (entity as IWithName).Name.ToLower().Contains(search))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        protected void AddDesignationSorting(int sortColumnId)
        {
            GridViewSettings.Sort.SetSortFunc(sortColumnId, (model, left, right) =>
            {
                var leftEntity = (IWithDesignation)model.GetValue(left, 0);
                var rightEntity = (IWithDesignation)model.GetValue(right, 0);

                if (leftEntity == null || rightEntity == null)
                {
                    return 0;
                }

                return leftEntity.Designation.CompareTo(rightEntity.Designation);
            });
        }
        protected void AddCodeSorting(int sortColumnId)
        {
            GridViewSettings.Sort.SetSortFunc(sortColumnId, (model, left, right) =>
            {
                var leftEntity = (IWithCode)model.GetValue(left, 0);
                var rightEnity = (IWithCode)model.GetValue(right, 0);

                if (leftEntity == null || rightEnity == null)
                {
                    return 0;
                }

                return leftEntity.Code.CompareTo(rightEnity.Code);
            });
        }
        protected void AddUpdatedAtSorting(int sortColumnId)
        {
            GridViewSettings.Sort.SetSortFunc(sortColumnId, (model, left, right) =>
            {
                var leftEntity = (ApiEntity)model.GetValue(left, 0);
                var rightEntity = (ApiEntity)model.GetValue(right, 0);

                if (leftEntity == null || rightEntity == null)
                {
                    return 0;
                }

                return leftEntity.UpdatedAt.CompareTo(rightEntity.UpdatedAt);
            });
        }

        protected abstract void InitializeSort();

        public virtual void Search(string searchText)
        {
            GridViewSettings.Filter.Refilter();
            this.Navigator.Update();
            
        }

        protected abstract void AddColumns();

        public abstract int RunModal(EntityEditionModalMode mode);

        protected virtual void Design()
        {
            VBox verticalLayout = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            verticalLayout.PackStart(scrolledWindow, true, true, 0);

            if (IsSelectionPage() == false)
            {
                verticalLayout.PackStart(Navigator, false, false, 0);
            }

            PackStart(verticalLayout);
        }

        public bool IsSelectionPage()
        {
            return Options != null && Options.ContainsKey("selection-page");
        }

        protected virtual void GridViewRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
                Navigator.CurrentRecord = Convert.ToInt16(GridViewSettings.Path.ToString());
                SelectedEntity = (TEntity)model.GetValue(GridViewSettings.Iterator, 0);
                EntitySelected?.Invoke(SelectedEntity);
            }
            ;

            Navigator.Update();
        }

        protected virtual void AddGridViewEventHandlers()
        {
            GridView.CursorChanged += GridViewRow_Changed;
            GridView.RowActivated += GridView_RowActivated;
            GridView.Vadjustment.ValueChanged += delegate { Navigator.Update(); };
            GridView.Vadjustment.Changed += delegate { Navigator.Update(); };
        }

        private void GridView_RowActivated(object o, RowActivatedArgs args)
        {
            if (IsSelectionPage())
            {
                RunModal(EntityEditionModalMode.View);
                return;
            }

            var response = (ResponseType)RunModal(EntityEditionModalMode.Update);

            if (response == ResponseType.Ok)
            {
                Refresh();
            }
        }
        
        protected PaginatedResult<TEntity>? ShowMore(
            PaginationQuery<TEntity> query)
        {

            query.GoToNextPage();

            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                query.GoToPreviousPage();
                ErrorHandlingService.HandleApiError(result,
                                                    source: SourceWindow);
                return null;
            }

            return result.Value;
        }

        public abstract void UpdateButtonPrevileges();
    }
}
