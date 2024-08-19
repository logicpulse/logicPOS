using Gtk;
using LogicPOS.Api.Errors;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public abstract class Page : Box
    {
        protected readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        public Window PageParentWindow { get; }
        public TreeView GridView { get; set; }
        public object SelectedEntity { get; set; }
        public bool CanViewEntity { get; set; }
        public bool CanUpdateEntity { get; set; }
        public bool CanDeleteEntity { get; set; }
        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();
        internal PageNavigator Navigator { get; }
        protected Dictionary<string,string> Options { get; set; }

        public Page(Window parent, Dictionary<string,string> options = null)
        {
            PageParentWindow = parent;
            Navigator = new PageNavigator(this);
            Options = options;

            LoadEntities();

            InitializeGridView();

            Design();

            AddEntitiesToModel();

            ShowAll();
        }

        protected void ShowApiErrorAlert()
        {
            SimpleAlerts.Error()
                .WithParent(PageParentWindow)
                .WithTitle("API")
                .WithMessage(ApiErrors.CommunicationError.Description)
                .Show();
        }

        public virtual void Refresh()
        {
            LoadEntities();
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            AddEntitiesToModel();
        }

        protected abstract ListStore CreateGridViewModel();
        public virtual void ViewEntity() => RunModal(EntityModalMode.View);
        public virtual void UpdateEntity() => RunModal(EntityModalMode.Update);
        public abstract void DeleteEntity();
        public virtual void InsertEntity() => RunModal(EntityModalMode.Insert);
        protected abstract void LoadEntities();
        protected virtual void InitializeGridView()
        {
            GridViewSettings.Model = CreateGridViewModel();

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddEventHandlers();
        }
        protected void InitializeGridViewModel()
        {
            InitializeFilter();
            InitializeSort();
        }

        protected abstract void InitializeSort();
        protected abstract void InitializeFilter();
        protected abstract void AddColumns();
        protected abstract void RunModal(EntityModalMode mode);
        protected virtual void Design()
        {
            VBox verticalLayout = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            verticalLayout.PackStart(scrolledWindow, true, true, 0);
            verticalLayout.PackStart(Navigator, false, false, 0);

            PackStart(verticalLayout);
        }

        protected abstract void AddEntitiesToModel();

        protected virtual void GridViewRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
                Navigator.CurrentRecord = Convert.ToInt16(GridViewSettings.Path.ToString());
                SelectedEntity = model.GetValue(GridViewSettings.Iterator, 0); ;
            };

            Navigator.Update();
        }

        protected virtual void AddEventHandlers()
        {
            GridView.CursorChanged += GridViewRow_Changed;
            GridView.RowActivated += delegate { UpdateEntity(); };
            GridView.Vadjustment.ValueChanged += delegate { Navigator.Update(); };
            GridView.Vadjustment.Changed += delegate { Navigator.Update(); };
        }
    }
}
